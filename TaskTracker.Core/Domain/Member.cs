namespace TaskTracker.Core.Domain;

/// <summary>Учасник команди, якому призначають завдання.</summary>
public sealed class Member
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsLead { get; init; }
}
