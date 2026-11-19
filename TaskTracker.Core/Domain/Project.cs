namespace TaskTracker.Core.Domain;

/// <summary>Проєкт команди, у межах якого ведуться завдання.</summary>
public sealed class Project
{
    /// <summary>Код проєкту.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Назва проєкту.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Погодинна ставка проєкту, грн.</summary>
    public decimal HourlyRate { get; init; }

    /// <summary>Кількість відкритих завдань проєкту.</summary>
    public int OpenTasks { get; init; }
}
