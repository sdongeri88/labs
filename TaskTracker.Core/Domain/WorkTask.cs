namespace TaskTracker.Core.Domain;

/// <summary>Робоче завдання, призначене учаснику команди.</summary>
public sealed class WorkTask
{
    private readonly List<WorkLog> _logs = new();

    /// <summary>Ідентифікатор завдання.</summary>
    public int Id { get; init; }

    /// <summary>Код проєкту, до якого належить завдання.</summary>
    public string ProjectCode { get; init; } = string.Empty;

    /// <summary>Ідентифікатор виконавця.</summary>
    public int AssigneeId { get; init; }

    /// <summary>Поточний стан завдання.</summary>
    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.New;

    /// <summary>Дата й час створення завдання.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Записи витраченого часу.</summary>
    public IReadOnlyList<WorkLog> Logs => _logs;

    /// <summary>Додає запис часу до завдання.</summary>
    /// <param name="log">Запис витраченого часу.</param>
    public void AddLog(WorkLog log) => _logs.Add(log);

    /// <summary>Обчислює сумарну кількість витрачених годин.</summary>
    /// <returns>Сума годин усіх записів часу.</returns>
    public decimal TotalHours()
    {
        decimal sum = 0m;
        foreach (WorkLog log in _logs)
        {
            sum += log.Hours;
        }

        return sum;
    }

    /// <summary>Обчислює вартість завдання: сума годин × ставка по всіх записах.</summary>
    /// <returns>Загальна вартість робіт за завданням.</returns>
    public decimal TotalCost()
    {
        decimal sum = 0m;
        foreach (WorkLog log in _logs)
        {
            sum += log.Cost;
        }

        return sum;
    }
}
