namespace TaskTracker.Core.Domain;

/// <summary>Учасник команди, якому призначають завдання.</summary>
public sealed class Member
{
    /// <summary>Ідентифікатор учасника.</summary>
    public int Id { get; init; }

    /// <summary>Ім’я учасника команди.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Ознака керівника команди.</summary>
    public bool IsLead { get; init; }
}
