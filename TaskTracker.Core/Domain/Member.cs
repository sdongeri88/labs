namespace TaskTracker.Core.Domain;

/// <summary>Учасник команди — незмінний запис довідника.</summary>
public sealed record Member
{
    /// <summary>Створює учасника з перевіркою інваріанта.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="name">Ім’я, не порожнє.</param>
    /// <param name="email">Пошта з символом @.</param>
    public Member(int id, string name, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }

    /// <summary>Ідентифікатор учасника.</summary>
    public int Id { get; }

    /// <summary>Ім’я учасника.</summary>
    public string Name { get; }

    /// <summary>Пошта учасника.</summary>
    public string Email { get; }

    /// <summary>Ознака керівника команди (необов’язкова, задається в ініціалізаторі).</summary>
    public bool IsLead { get; init; }
}
