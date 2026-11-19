namespace TaskTracker.Core.Domain;

/// <summary>
/// Робоче завдання — агрегат зі станом. Записи часу й стан змінюються
/// лише методами-переходами, що перевіряють дозволеність переходу.
/// </summary>
public sealed class WorkTask
{
    private readonly List<WorkLog> _logs = new();

    /// <summary>Створює нове завдання у стані <see cref="WorkTaskStatus.New"/>.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="projectCode">Код проєкту, не порожній.</param>
    /// <param name="assigneeId">Ідентифікатор виконавця, більший за нуль.</param>
    /// <param name="createdAt">Дата створення.</param>
    public WorkTask(int id, string projectCode, int assigneeId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(assigneeId);

        Id = id;
        ProjectCode = projectCode;
        AssigneeId = assigneeId;
        CreatedAt = createdAt;
        Status = WorkTaskStatus.New;
    }

    /// <summary>Ідентифікатор завдання.</summary>
    public int Id { get; }

    /// <summary>Код проєкту.</summary>
    public string ProjectCode { get; }

    /// <summary>Ідентифікатор виконавця.</summary>
    public int AssigneeId { get; }

    /// <summary>Дата створення.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Поточний стан; змінюється лише методами-переходами.</summary>
    public WorkTaskStatus Status { get; private set; }

    /// <summary>Записи часу: видно всім, додати можна лише через <see cref="AddLog"/>.</summary>
    public IReadOnlyList<WorkLog> Logs => _logs;

    /// <summary>Сумарна кількість годин (обчислюється з записів).</summary>
    public decimal TotalHours => _logs.Sum(log => log.Hours);

    /// <summary>Додає запис часу; дозволено лише в стані InProgress.</summary>
    /// <param name="log">Запис часу.</param>
    public void AddLog(WorkLog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        EnsureStatus(WorkTaskStatus.InProgress, "Час обліковують лише для завдання в роботі.");
        _logs.Add(log);
    }

    /// <summary>Бере завдання в роботу: New → InProgress.</summary>
    public void Start()
    {
        EnsureStatus(WorkTaskStatus.New, "Почати можна лише нове завдання.");
        Status = WorkTaskStatus.InProgress;
    }

    /// <summary>Передає на перевірку: InProgress → Review (потрібен запис часу).</summary>
    public void SubmitForReview()
    {
        EnsureStatus(WorkTaskStatus.InProgress, "На перевірку передають завдання в роботі.");
        if (_logs.Count == 0)
        {
            throw new InvalidOperationException("Завдання без записів часу не можна передати.");
        }

        Status = WorkTaskStatus.Review;
    }

    /// <summary>Завершує завдання: Review → Done.</summary>
    public void Complete()
    {
        EnsureStatus(WorkTaskStatus.Review, "Завершити можна лише перевірене завдання.");
        Status = WorkTaskStatus.Done;
    }

    /// <summary>Повертає на доопрацювання: Review → InProgress.</summary>
    public void ReturnToWork()
    {
        EnsureStatus(WorkTaskStatus.Review, "Повернути можна лише завдання на перевірці.");
        Status = WorkTaskStatus.InProgress;
    }

    /// <summary>Скасовує завдання зі стану New або InProgress.</summary>
    public void Cancel()
    {
        if (Status is not (WorkTaskStatus.New or WorkTaskStatus.InProgress))
        {
            throw new InvalidOperationException("Скасувати можна лише нове або активне завдання.");
        }

        Status = WorkTaskStatus.Cancelled;
    }

    private void EnsureStatus(WorkTaskStatus expected, string message)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException(message);
        }
    }
}
