namespace TaskTracker.Domain;

/// <summary>Рядок звіту «завантаження виконавців».</summary>
/// <param name="Login">Логін виконавця.</param>
/// <param name="Name">Ім'я виконавця.</param>
/// <param name="ActiveTasks">Кількість завдань у InProgress і Review.</param>
/// <param name="ActiveHours">Сума оцінок активних завдань, годин.</param>
/// <param name="CapacityHours">Місткість на тиждень, годин.</param>
/// <param name="LoadPercent">Завантаження, % (ціле, округлення від нуля).</param>
/// <param name="Level">Рівень завантаження.</param>
public sealed record MemberWorkload(
    string Login,
    string Name,
    int ActiveTasks,
    decimal ActiveHours,
    decimal CapacityHours,
    decimal LoadPercent,
    WorkloadLevel Level);
