using TaskTracker.Domain;

namespace TaskTracker.Infrastructure.Seed;

/// <summary>Умовні дані дошки для першого запуску.</summary>
/// <param name="Members">Учасники команди.</param>
/// <param name="Tasks">Завдання дошки.</param>
public sealed record DemoBoard(IReadOnlyList<Member> Members, IReadOnlyList<BoardTask> Tasks);
