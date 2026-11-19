namespace TaskTracker.Core.Domain;

/// <summary>
/// Запис витраченого часу — незмінна позиція завдання.
/// Створений запис зіпсувати неможливо: усі перевірки — у конструкторі.
/// </summary>
public sealed record WorkLog
{
    /// <summary>Найбільша кількість годин в одному записі (одна доба).</summary>
    public const decimal MaxHoursPerEntry = 24m;

    /// <summary>Створює запис часу з перевіркою інваріанта.</summary>
    /// <param name="activity">Вид роботи, не порожній.</param>
    /// <param name="hours">Години: більше 0 і не більше 24.</param>
    /// <param name="hourlyRate">Погодинна ставка, не від’ємна.</param>
    public WorkLog(string activity, decimal hours, decimal hourlyRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(activity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hours);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hours, MaxHoursPerEntry);
        ArgumentOutOfRangeException.ThrowIfNegative(hourlyRate);

        Activity = activity;
        Hours = hours;
        HourlyRate = hourlyRate;
    }

    /// <summary>Вид роботи.</summary>
    public string Activity { get; }

    /// <summary>Кількість витрачених годин.</summary>
    public decimal Hours { get; }

    /// <summary>Погодинна ставка, грн.</summary>
    public decimal HourlyRate { get; }

    /// <summary>Вартість: обчислюється, тому не розійдеться з годинами й ставкою.</summary>
    public decimal Cost => Hours * HourlyRate;
}
