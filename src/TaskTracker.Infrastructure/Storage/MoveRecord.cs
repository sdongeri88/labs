using TaskTracker.Domain;

namespace TaskTracker.Infrastructure.Storage;

/// <summary>Серіалізаційний запис одного переміщення.</summary>
/// <param name="From">Колонка до переміщення.</param>
/// <param name="To">Колонка після переміщення.</param>
/// <param name="At">Момент переміщення.</param>
internal sealed record MoveRecord(BoardColumn From, BoardColumn To, DateTimeOffset At);
