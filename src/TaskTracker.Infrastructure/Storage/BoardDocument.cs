namespace TaskTracker.Infrastructure.Storage;

/// <summary>Вміст файлу дошки: довідник учасників і завдання.</summary>
internal sealed record BoardDocument
{
    /// <summary>Учасники команди.</summary>
    public IReadOnlyList<MemberRecord> Members { get; init; } = [];

    /// <summary>Завдання дошки.</summary>
    public IReadOnlyList<TaskRecord> Tasks { get; init; } = [];
}
