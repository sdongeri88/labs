namespace TaskTracker.Domain;

/// <summary>Пріоритет завдання; менше число означає вищий пріоритет.</summary>
public enum TaskPriority
{
    /// <summary>Критичний: блокує роботу інших.</summary>
    Critical = 1,

    /// <summary>Високий.</summary>
    High = 2,

    /// <summary>Звичайний.</summary>
    Normal = 3,

    /// <summary>Низький.</summary>
    Low = 4,
}
