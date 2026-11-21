namespace TaskTracker.Domain;

/// <summary>Змістові дані нового завдання (об'єкт-параметр конструктора).</summary>
/// <param name="Title">Назва, не порожня.</param>
/// <param name="Priority">Пріоритет.</param>
/// <param name="EstimateHours">Оцінка трудомісткості, годин; не від'ємна.</param>
public sealed record TaskDetails(string Title, TaskPriority Priority, decimal EstimateHours);
