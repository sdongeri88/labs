namespace TaskTracker.Infrastructure.Storage;

/// <summary>Серіалізаційний запис учасника команди.</summary>
/// <param name="Login">Логін.</param>
/// <param name="Name">Ім'я.</param>
/// <param name="WeeklyCapacityHours">Місткість на тиждень, годин.</param>
internal sealed record MemberRecord(string Login, string Name, decimal WeeklyCapacityHours);
