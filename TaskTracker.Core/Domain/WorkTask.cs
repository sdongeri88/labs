namespace TaskTracker.Core.Domain;

/// <summary>Робоче завдання, призначене учаснику команди.</summary>
public sealed class WorkTask
{
    private readonly List<WorkLog> _logs = new();

    public int Id { get; init; }

    public string ProjectCode { get; init; } = string.Empty;

    public int AssigneeId { get; init; }

    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<WorkLog> Logs => _logs;

    /// <summary>Додає запис часу до завдання.</summary>
    public void AddLog(WorkLog log) => _logs.Add(log);

    /// <summary>Обчислює сумарну кількість витрачених годин.</summary>
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
