namespace TaskTracker.Core.Domain;

/// <summary>Проєкт команди, у межах якого ведуться завдання.</summary>
public sealed class Project
{
    public string Code { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public decimal HourlyRate { get; init; }

    public int OpenTasks { get; init; }
}
