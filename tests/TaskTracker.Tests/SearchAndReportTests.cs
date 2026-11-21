using TaskTracker.Application.Services;
using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Пошук UC-04 і звіт UC-05: нуль, один і кілька записів у правильному порядку.</summary>
public class SearchAndReportTests
{
    [Fact]
    public void Find_NoMatches_ReturnsEmpty()
    {
        TaskService service = NewService(Tasks.In(BoardColumn.Todo, "ivan"));

        Result<IReadOnlyList<BoardTask>> found = service.Find(new TaskQuery("olena", null, null));

        Assert.Empty(found.Value!);
    }

    [Fact]
    public void Find_AssigneeAndColumn_ReturnsSingleMatch()
    {
        BoardTask expected = Tasks.In(BoardColumn.Review, "ivan");
        TaskService service = NewService(Tasks.In(BoardColumn.Todo, "ivan"), expected);

        Result<IReadOnlyList<BoardTask>> found = service.Find(new TaskQuery("Ivan", "review", ""));

        Assert.Same(expected, Assert.Single(found.Value!));
    }

    [Fact]
    public void Find_SeveralMatches_SortedByPriorityThenId()
    {
        BoardTask low = NewTask(3, TaskPriority.Low);
        BoardTask critical = NewTask(5, TaskPriority.Critical);
        BoardTask highFirst = NewTask(1, TaskPriority.High);
        BoardTask highSecond = NewTask(2, TaskPriority.High);
        TaskService service = NewService(low, critical, highSecond, highFirst);

        Result<IReadOnlyList<BoardTask>> found = service.Find(new TaskQuery(null, null, "звіт"));

        Assert.Equal([5, 1, 2, 3], found.Value!.Select(t => t.Id));
    }

    [Fact]
    public void Find_UnknownColumn_FailsWithMessage()
    {
        TaskService service = NewService();

        Result<IReadOnlyList<BoardTask>> found = service.Find(new TaskQuery(null, "Archive", null));

        Assert.Equal("Невідома колонка «Archive».", found.Error);
    }

    [Fact]
    public void Build_SeveralMembers_SortedByLoadThenLogin()
    {
        var members = new FakeMemberDirectory(
            new Member("olena", "Олена", 10m),
            new Member("ivan", "Іван", 10m),
            new Member("anna", "Анна", 10m));
        var tasks = new FakeTaskRepository(
            Tasks.In(BoardColumn.InProgress, "ivan", 5m),
            Tasks.In(BoardColumn.InProgress, "olena", 5m));
        var service = new WorkloadReportService(tasks, members, new FixedClock());

        WorkloadReport report = service.Build();

        Assert.Equal(["ivan", "olena", "anna"], report.Rows.Select(r => r.Login));
        Assert.Equal(33m, report.TeamLoadPercent);
    }

    [Fact]
    public void Build_NoMembers_ReturnsEmptyReportWithZeroLoad()
    {
        var service = new WorkloadReportService(
            new FakeTaskRepository(Tasks.In(BoardColumn.InProgress, "ivan")),
            new FakeMemberDirectory(),
            new FixedClock());

        WorkloadReport report = service.Build();

        Assert.Empty(report.Rows);
        Assert.Equal(0m, report.TeamLoadPercent);
    }

    private static BoardTask NewTask(int id, TaskPriority priority) =>
        new(id, new TaskDetails("Звіт " + id, priority, 1m), new DateOnly(2026, 11, 1));

    private static TaskService NewService(params BoardTask[] tasks) =>
        new(
            new FakeTaskRepository(tasks),
            new FakeMemberDirectory(),
            new FixedClock(),
            new SpyLogger<TaskService>());
}
