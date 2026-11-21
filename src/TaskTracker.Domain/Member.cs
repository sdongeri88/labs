namespace TaskTracker.Domain;

/// <summary>Учасник команди, якому призначають завдання.</summary>
public sealed class Member
{
    /// <summary>Створює учасника з перевіркою інваріантів.</summary>
    /// <param name="login">Логін; зберігається в нижньому регістрі.</param>
    /// <param name="name">Повне ім'я для звітів.</param>
    /// <param name="weeklyCapacityHours">Місткість на тиждень, годин; більша за нуль.</param>
    /// <exception cref="ArgumentException">Порожній логін або ім'я.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Місткість не додатна.</exception>
    public Member(string login, string name, decimal weeklyCapacityHours)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weeklyCapacityHours);

        Login = login.Trim().ToLowerInvariant();
        Name = name.Trim();
        WeeklyCapacityHours = weeklyCapacityHours;
    }

    /// <summary>Логін у нижньому регістрі; унікальний у команді.</summary>
    public string Login { get; }

    /// <summary>Повне ім'я.</summary>
    public string Name { get; }

    /// <summary>Скільки годин на тиждень учасник може віддати завданням дошки.</summary>
    public decimal WeeklyCapacityHours { get; }
}
