namespace TaskTracker.Core.Requests;

/// <summary>Дані клієнта, який подав заявку.</summary>
/// <param name="Name">Ім’я клієнта.</param>
/// <param name="Email">Пошта клієнта.</param>
/// <param name="IsRegular">Ознака постійного клієнта.</param>
public sealed record ClientInfo(
    string Name,
    string Email,
    bool IsRegular);
