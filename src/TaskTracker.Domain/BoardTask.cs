namespace TaskTracker.Domain;

/// <summary>
/// Завдання на дошці — документ зі станом. Колонка змінюється лише методом
/// <see cref="MoveTo"/>, який перевіряє профільне правило.
/// </summary>
public sealed class BoardTask
{
    private readonly List<TaskMove> _history = [];

    /// <summary>Створює завдання в колонці Backlog.</summary>
    /// <param name="id">Номер завдання, більший за нуль.</param>
    /// <param name="details">Назва, пріоритет і оцінка.</param>
    /// <param name="createdOn">Дата створення.</param>
    /// <exception cref="ArgumentException">Порожня назва.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Номер або оцінка поза межами.</exception>
    public BoardTask(int id, TaskDetails details, DateOnly createdOn)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(details);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.Title);
        ArgumentOutOfRangeException.ThrowIfNegative(details.EstimateHours);

        Id = id;
        Title = details.Title.Trim();
        Priority = details.Priority;
        EstimateHours = details.EstimateHours;
        CreatedOn = createdOn;
        Column = BoardColumn.Backlog;
    }

    /// <summary>Номер завдання.</summary>
    public int Id { get; }

    /// <summary>Назва завдання.</summary>
    public string Title { get; }

    /// <summary>Пріоритет.</summary>
    public TaskPriority Priority { get; }

    /// <summary>Оцінка трудомісткості, годин.</summary>
    public decimal EstimateHours { get; }

    /// <summary>Дата створення.</summary>
    public DateOnly CreatedOn { get; }

    /// <summary>Логін виконавця або null, якщо завдання ще нікому не призначене.</summary>
    public string? AssigneeLogin { get; private set; }

    /// <summary>Поточна колонка.</summary>
    public BoardColumn Column { get; private set; }

    /// <summary>Чи враховується завдання в особистому ліміті (InProgress або Review).</summary>
    public bool IsActive => Column is BoardColumn.InProgress or BoardColumn.Review;

    /// <summary>Історія переміщень у хронологічному порядку.</summary>
    public IReadOnlyList<TaskMove> History => _history;

    /// <summary>Відновлює завдання зі сховища в заданій колонці без перевірки правил.</summary>
    /// <param name="column">Збережена колонка.</param>
    /// <param name="assigneeLogin">Збережений виконавець або null.</param>
    /// <param name="history">Збережена історія переміщень.</param>
    public void Restore(BoardColumn column, string? assigneeLogin, IEnumerable<TaskMove> history)
    {
        ArgumentNullException.ThrowIfNull(history);
        Column = column;
        AssigneeLogin = assigneeLogin;
        _history.Clear();
        _history.AddRange(history);
    }

    /// <summary>Призначає виконавця; у кінцевій колонці Done призначення заборонене.</summary>
    /// <param name="login">Логін виконавця.</param>
    /// <exception cref="DomainException">Завдання вже виконано.</exception>
    public void AssignTo(string login)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        if (Column == BoardColumn.Done)
        {
            throw new DomainException($"Завдання #{Id} уже виконано, виконавця не змінюють.");
        }

        AssigneeLogin = login;
    }

    /// <summary>Переміщує завдання, якщо правило дозволяє, і дописує історію.</summary>
    /// <param name="target">Цільова колонка.</param>
    /// <param name="policy">Профільне правило переміщення.</param>
    /// <param name="board">Усі завдання дошки.</param>
    /// <param name="at">Момент переміщення.</param>
    /// <exception cref="MoveRejectedException">Правило відхилило переміщення.</exception>
    public void MoveTo(
        BoardColumn target,
        IMovePolicy policy,
        IReadOnlyCollection<BoardTask> board,
        DateTimeOffset at)
    {
        ArgumentNullException.ThrowIfNull(policy);
        MoveVerdict verdict = policy.Check(this, target, board);
        if (verdict != MoveVerdict.Allowed)
        {
            throw new MoveRejectedException(verdict, RejectionText(verdict, target));
        }

        _history.Add(new TaskMove(Column, target, at));
        Column = target;
    }

    private string RejectionText(MoveVerdict verdict, BoardColumn target) => verdict switch
    {
        MoveVerdict.ForbiddenTransition => $"Перехід {Column} → {target} заборонено для #{Id}.",
        MoveVerdict.AssigneeRequired => $"Завдання #{Id} без виконавця не можна взяти в роботу.",
        MoveVerdict.ColumnLimitReached => $"Колонка {target} заповнена до ліміту WIP.",
        MoveVerdict.PersonalLimitReached => $"У {AssigneeLogin} уже максимум активних завдань.",
        _ => $"Переміщення #{Id} відхилено.",
    };
}
