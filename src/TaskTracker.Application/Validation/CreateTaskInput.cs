namespace TaskTracker.Application.Validation;

/// <summary>Сирий ввід сценарію UC-01: рядки так, як їх набрав користувач.</summary>
/// <param name="Title">Назва завдання.</param>
/// <param name="Priority">Пріоритет: 1–4 або назва (Critical, High, Normal, Low).</param>
/// <param name="EstimateHours">Оцінка в годинах; дробова частина через кому або крапку.</param>
/// <param name="Assignee">Логін виконавця; порожньо — без виконавця.</param>
public sealed record CreateTaskInput(
    string? Title, string? Priority, string? EstimateHours, string? Assignee);
