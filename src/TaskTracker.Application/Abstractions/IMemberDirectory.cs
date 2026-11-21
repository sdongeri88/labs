using TaskTracker.Domain;

namespace TaskTracker.Application.Abstractions;

/// <summary>Довідник учасників команди.</summary>
public interface IMemberDirectory
{
    /// <summary>Повертає всіх учасників.</summary>
    /// <returns>Знімок довідника.</returns>
    IReadOnlyList<Member> GetAll();

    /// <summary>Шукає учасника за логіном без урахування регістру.</summary>
    /// <param name="login">Логін.</param>
    /// <returns>Учасник або null.</returns>
    Member? FindByLogin(string login);
}
