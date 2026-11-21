namespace TaskTracker.Domain;

/// <summary>Запис історії: завдання переміщено з однієї колонки до іншої.</summary>
/// <param name="From">Колонка до переміщення.</param>
/// <param name="To">Колонка після переміщення.</param>
/// <param name="At">Момент переміщення.</param>
public sealed record TaskMove(BoardColumn From, BoardColumn To, DateTimeOffset At);
