namespace TaskTracker.Core.Domain;

/// <summary>
/// Архівне завдання НЕ є завданням: воно містить завдання й показує лише
/// його дані. Методу додавання записів немає — порушити контракт нічим.
/// </summary>
public sealed class ArchivedWorkTask
{
    private readonly WorkTask _task;

    /// <summary>Архівує завершене або скасоване завдання.</summary>
    /// <param name="task">Завдання в кінцевому стані.</param>
    /// <param name="archivedAt">Дата архівації.</param>
    public ArchivedWorkTask(WorkTask task, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (task.Status is not (WorkTaskStatus.Done or WorkTaskStatus.Cancelled))
        {
            throw new ArgumentException("Архівують лише завершені чи скасовані.", nameof(task));
        }

        _task = task;
        ArchivedAt = archivedAt;
    }

    /// <summary>Дата архівації.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Ідентифікатор завдання.</summary>
    public int Id => _task.Id;

    /// <summary>Кінцевий стан завдання.</summary>
    public WorkTaskStatus Status => _task.Status;

    /// <summary>Записи часу (лише для читання).</summary>
    public IReadOnlyList<WorkLog> Logs => _task.Logs;
}
