namespace TaskTracker.Application.Validation;

/// <summary>Сирий ввід пошуку UC-04; порожній критерій не обмежує вибірку.</summary>
/// <param name="Assignee">Логін виконавця.</param>
/// <param name="Column">Колонка дошки.</param>
/// <param name="TitleFragment">Фрагмент назви.</param>
public sealed record TaskQuery(string? Assignee, string? Column, string? TitleFragment);
