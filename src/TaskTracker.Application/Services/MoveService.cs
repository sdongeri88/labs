using Microsoft.Extensions.Logging;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Application.Services;

/// <summary>Сценарій UC-02: переміщення завдання дошкою за профільним правилом.</summary>
public sealed class MoveService
{
    private readonly ITaskRepository _tasks;
    private readonly IMovePolicy _policy;
    private readonly IClock _clock;
    private readonly ILogger<MoveService> _log;

    /// <summary>Створює сервіс із залежностями, переданими ззовні.</summary>
    /// <param name="tasks">Сховище завдань.</param>
    /// <param name="policy">Профільне правило переміщення.</param>
    /// <param name="clock">Годинник для історії переміщень.</param>
    /// <param name="log">Журнал сервісу.</param>
    public MoveService(
        ITaskRepository tasks, IMovePolicy policy, IClock clock, ILogger<MoveService> log)
    {
        _tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>UC-02. Переміщує завдання до колонки.</summary>
    /// <param name="taskId">Номер завдання (рядок із консолі).</param>
    /// <param name="column">Назва цільової колонки.</param>
    /// <returns>Переміщене завдання або пояснення некоректного вводу.</returns>
    /// <exception cref="MoveRejectedException">Правило відхилило переміщення.</exception>
    public Result<BoardTask> Move(string? taskId, string? column)
    {
        using IDisposable? scope = _log.BeginScope(LogText.Scope, "UC-02");
        _log.LogInformation(LogText.Started);

        Result<int> id = InputParser.TaskId(taskId);
        Result<BoardColumn> target = InputParser.Column(column);
        string? error = !id.IsSuccess ? id.Error : (!target.IsSuccess ? target.Error : null);
        BoardTask? task = error is null ? _tasks.FindById(id.Value) : null;
        if (error is null && task is null)
        {
            error = $"Завдання #{id.Value} не знайдено.";
        }

        if (error is not null)
        {
            _log.LogWarning(LogText.Rejected, error);
            return Result.Fail<BoardTask>(error);
        }

        Apply(task!, target.Value);
        _log.LogInformation(LogText.Finished, task!.Id);
        return Result.Ok(task!);
    }

    private void Apply(BoardTask task, BoardColumn target)
    {
        try
        {
            task.MoveTo(target, _policy, _tasks.GetAll().ToList(), _clock.Now);
        }
        catch (MoveRejectedException ex)
        {
            _log.LogWarning(LogText.Rejected, ex.Verdict);
            throw;
        }
    }
}
