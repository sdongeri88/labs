namespace TaskTracker.Core.Domain;

/// <summary>Стан робочого завдання в його життєвому циклі.</summary>
public enum WorkTaskStatus
{
    /// <summary>Завдання створено.</summary>
    New = 0,

    /// <summary>Завдання в роботі.</summary>
    InProgress = 1,

    /// <summary>Завдання на перевірці.</summary>
    Review = 2,

    /// <summary>Завдання виконано.</summary>
    Done = 3,

    /// <summary>Завдання скасовано.</summary>
    Cancelled = 4,
}
