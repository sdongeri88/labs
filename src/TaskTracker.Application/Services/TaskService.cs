using Microsoft.Extensions.Logging;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Application.Services;

/// <summary>Сценарії UC-01, UC-03, UC-04, UC-06: створити, призначити, знайти, зберегти.</summary>
public sealed class TaskService
{
    private readonly ITaskRepository _tasks;
    private readonly IMemberDirectory _members;
    private readonly IClock _clock;
    private readonly ILogger<TaskService> _log;

    /// <summary>Створює сервіс із залежностями, переданими ззовні.</summary>
    /// <param name="tasks">Сховище завдань.</param>
    /// <param name="members">Довідник учасників.</param>
    /// <param name="clock">Годинник.</param>
    /// <param name="log">Журнал сервісу.</param>
    public TaskService(
        ITaskRepository tasks, IMemberDirectory members, IClock clock, ILogger<TaskService> log)
    {
        _tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        _members = members ?? throw new ArgumentNullException(nameof(members));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>UC-01. Створює завдання в Backlog, за потреби одразу з виконавцем.</summary>
    /// <param name="input">Сирий ввід користувача.</param>
    /// <returns>Створене завдання або пояснення, чому ввід відхилено.</returns>
    public Result<BoardTask> Create(CreateTaskInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        using IDisposable? scope = _log.BeginScope(LogText.Scope, "UC-01");
        _log.LogInformation(LogText.Started);

        Result<TaskDetails> details = TaskInputValidator.Validate(input);
        Result<Member?> assignee = details.IsSuccess
            ? FindOptionalMember(input.Assignee)
            : Result.Fail<Member?>(details.Error);
        if (!assignee.IsSuccess)
        {
            _log.LogWarning(LogText.Rejected, assignee.Error);
            return Result.Fail<BoardTask>(assignee.Error);
        }

        var task = new BoardTask(_tasks.NextId(), details.Value!, _clock.Today);
        if (assignee.Value is not null)
        {
            task.AssignTo(assignee.Value.Login);
        }

        _tasks.Add(task);
        _log.LogInformation(LogText.Finished, task.Id);
        return Result.Ok(task);
    }

    /// <summary>UC-03. Призначає завданню виконавця з довідника.</summary>
    /// <param name="taskId">Номер завдання (рядок із консолі).</param>
    /// <param name="login">Логін виконавця.</param>
    /// <returns>Завдання з новим виконавцем або пояснення.</returns>
    /// <exception cref="DomainException">Завдання вже виконано.</exception>
    public Result<BoardTask> Assign(string? taskId, string? login)
    {
        using IDisposable? scope = _log.BeginScope(LogText.Scope, "UC-03");
        _log.LogInformation(LogText.Started);

        Result<BoardTask> task = FindTask(taskId);
        Result<Member?> member = task.IsSuccess
            ? FindOptionalMember(login)
            : Result.Fail<Member?>(task.Error);
        if (member.IsSuccess && member.Value is null)
        {
            member = Result.Fail<Member?>("Вкажіть логін виконавця.");
        }

        if (!member.IsSuccess)
        {
            _log.LogWarning(LogText.Rejected, member.Error);
            return Result.Fail<BoardTask>(member.Error);
        }

        AssignOrLog(task.Value!, member.Value!.Login);
        _log.LogInformation(LogText.Finished, task.Value!.Id);
        return task;
    }

    /// <summary>UC-04. Шукає завдання за виконавцем, колонкою і фрагментом назви.</summary>
    /// <param name="query">Критерії; порожній критерій не обмежує вибірку.</param>
    /// <returns>Завдання за пріоритетом, потім за номером, або пояснення.</returns>
    public Result<IReadOnlyList<BoardTask>> Find(TaskQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        BoardColumn? column = null;
        if (!string.IsNullOrWhiteSpace(query.Column))
        {
            Result<BoardColumn> parsed = InputParser.Column(query.Column);
            if (!parsed.IsSuccess)
            {
                return Result.Fail<IReadOnlyList<BoardTask>>(parsed.Error);
            }

            column = parsed.Value;
        }

        IReadOnlyList<BoardTask> found = _tasks.GetAll()
            .Where(t => Matches(t, query, column))
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.Id)
            .ToList();
        return Result.Ok(found);
    }

    /// <summary>UC-06. Зберігає дошку у сховищі.</summary>
    /// <exception cref="IOException">Сховище недоступне; подію записано до журналу.</exception>
    public void Save()
    {
        using IDisposable? scope = _log.BeginScope(LogText.Scope, "UC-06");
        _log.LogInformation(LogText.Started);
        try
        {
            _tasks.SaveChanges();
        }
        catch (IOException ex)
        {
            _log.LogError(ex, LogText.StorageFailed, _tasks.GetAll().Count);
            throw;
        }

        _log.LogInformation(LogText.Saved, _tasks.GetAll().Count);
    }

    private static bool Matches(BoardTask task, TaskQuery query, BoardColumn? column)
    {
        bool byAssignee = string.IsNullOrWhiteSpace(query.Assignee)
            || string.Equals(
                task.AssigneeLogin, query.Assignee.Trim(), StringComparison.OrdinalIgnoreCase);
        bool byColumn = column is null || task.Column == column;
        bool byTitle = string.IsNullOrWhiteSpace(query.TitleFragment)
            || task.Title.Contains(query.TitleFragment.Trim(), StringComparison.OrdinalIgnoreCase);
        return byAssignee && byColumn && byTitle;
    }

    private Result<BoardTask> FindTask(string? taskId)
    {
        Result<int> id = InputParser.TaskId(taskId);
        if (!id.IsSuccess)
        {
            return Result.Fail<BoardTask>(id.Error);
        }

        BoardTask? task = _tasks.FindById(id.Value);
        return task is null
            ? Result.Fail<BoardTask>($"Завдання #{id.Value} не знайдено.")
            : Result.Ok(task);
    }

    private Result<Member?> FindOptionalMember(string? login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return Result.Ok<Member?>(null);
        }

        Member? member = _members.FindByLogin(login.Trim());
        return member is null
            ? Result.Fail<Member?>($"Виконавця «{login.Trim()}» немає в команді.")
            : Result.Ok<Member?>(member);
    }

    private void AssignOrLog(BoardTask task, string login)
    {
        try
        {
            task.AssignTo(login);
        }
        catch (DomainException ex)
        {
            _log.LogWarning(LogText.Rejected, ex.Message);
            throw;
        }
    }
}
