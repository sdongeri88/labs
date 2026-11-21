using Microsoft.Extensions.Logging;
using TaskTracker.Application.Services;
using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Сценарії UC-01, UC-03, UC-06 із фейковим сховищем і фіксованим годинником.</summary>
public class TaskServiceTests
{
    private static Member Ivan { get; } = new("ivan", "Іван Мороз", 30m);

    [Fact]
    public void Create_ValidInput_AddsTaskToBacklogWithAssignee()
    {
        var repository = new FakeTaskRepository(Tasks.In(BoardColumn.Done));
        TaskService service = NewService(repository);

        Result<BoardTask> result = service.Create(new CreateTaskInput("Звіт", "2", "3,5", "IVAN"));

        BoardTask task = Assert.IsType<BoardTask>(result.Value);
        Assert.Equal(
            (BoardColumn.Backlog, "ivan", 3.5m, TaskPriority.High, new DateOnly(2026, 11, 20)),
            (task.Column, task.AssigneeLogin, task.EstimateHours, task.Priority, task.CreatedOn));
        Assert.Equal(2, repository.GetAll().Count);
    }

    [Fact]
    public void Create_UnknownAssignee_FailsAndLogsWarning()
    {
        var repository = new FakeTaskRepository();
        var log = new SpyLogger<TaskService>();
        TaskService service = NewService(repository, log);

        Result<BoardTask> result = service.Create(new CreateTaskInput("Звіт", "", "1", "nobody"));

        Assert.False(result.IsSuccess);
        Assert.Empty(repository.GetAll());
        Assert.Contains(LogLevel.Warning, log.Levels);
    }

    [Fact]
    public void Create_EmptyTitle_FailsWithTitleMessage()
    {
        TaskService service = NewService(new FakeTaskRepository());

        Result<BoardTask> result = service.Create(new CreateTaskInput(" ", "1", "1", null));

        Assert.Equal("Назва не може бути порожньою.", result.Error);
    }

    [Theory]
    [InlineData("9", "1", "Невідомий пріоритет «9».")]
    [InlineData("1", "-2", "Оцінка має бути від 0 до 80 год.")]
    public void Create_BadPriorityOrHours_FailsWithMessage(
        string priority, string hours, string expected)
    {
        TaskService service = NewService(new FakeTaskRepository());

        var input = new CreateTaskInput("Звіт", priority, hours, null);

        Result<BoardTask> result = service.Create(input);

        Assert.Equal(expected, result.Error);
    }

    [Fact]
    public void Assign_ExistingTaskAndMember_SetsAssignee()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo);
        TaskService service = NewService(new FakeTaskRepository(task));

        Result<BoardTask> result = service.Assign(Tasks.IdOf(task), "Ivan");

        Assert.True(result.IsSuccess);
        Assert.Equal("ivan", task.AssigneeLogin);
    }

    [Theory]
    [InlineData("999", "ivan")]
    [InlineData("x", "ivan")]
    [InlineData(null, "")]
    public void Assign_UnknownTaskOrEmptyLogin_Fails(string? id, string login)
    {
        TaskService service = NewService(new FakeTaskRepository(Tasks.In(BoardColumn.Todo)));

        Assert.False(service.Assign(id, login).IsSuccess);
    }

    [Fact]
    public void Assign_EmptyLoginForExistingTask_AsksForLogin()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo);
        TaskService service = NewService(new FakeTaskRepository(task));

        Result<BoardTask> result = service.Assign(Tasks.IdOf(task), " ");

        Assert.Equal("Вкажіть логін виконавця.", result.Error);
    }

    [Fact]
    public void Assign_DoneTask_ThrowsDomainExceptionAndLogsWarning()
    {
        BoardTask task = Tasks.In(BoardColumn.Done, "olena");
        var log = new SpyLogger<TaskService>();
        TaskService service = NewService(new FakeTaskRepository(task), log);

        Assert.Throws<DomainException>(() => service.Assign(Tasks.IdOf(task), "ivan"));
        Assert.Contains(LogLevel.Warning, log.Levels);
    }

    [Fact]
    public void Save_StorageFails_LogsErrorAndRethrows()
    {
        var log = new SpyLogger<TaskService>();
        TaskService service = NewService(new FakeTaskRepository { FailOnSave = true }, log);

        Assert.Throws<IOException>(service.Save);
        Assert.Contains(LogLevel.Error, log.Levels);
    }

    [Fact]
    public void Save_StorageAvailable_SavesOnce()
    {
        var repository = new FakeTaskRepository();
        TaskService service = NewService(repository);

        service.Save();

        Assert.Equal(1, repository.SaveCalls);
    }

    private static TaskService NewService(
        FakeTaskRepository repository, SpyLogger<TaskService>? log = null) =>
        new(
            repository,
            new FakeMemberDirectory(Ivan),
            new FixedClock(),
            log ?? new SpyLogger<TaskService>());
}
