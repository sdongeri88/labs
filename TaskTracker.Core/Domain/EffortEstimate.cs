namespace TaskTracker.Core.Domain;

/// <summary>
/// Оцінка трудомісткості завдання в годинах: додатна, кратна кроку 0,25 год
/// і не більша за 80 год. Незмінна; некоректну оцінку створити неможливо.
/// </summary>
public sealed record EffortEstimate
{
    /// <summary>Крок оцінки — чверть години (15 хв), найменша одиниця планування.</summary>
    public const decimal StepHours = 0.25m;

    /// <summary>Найбільша оцінка завдання (два тижні); більші завдання декомпонують.</summary>
    public const decimal MaxHours = 80m;

    /// <summary>Частка оцінки, з якої завдання вважається близьким до межі.</summary>
    public const decimal NearLimitRatio = 0.8m;

    private EffortEstimate(decimal hours)
    {
        Hours = hours;
    }

    /// <summary>Оцінка в годинах, кратна <see cref="StepHours"/>.</summary>
    public decimal Hours { get; }

    /// <summary>
    /// Створює оцінку, округлюючи години вгору до кроку 0,25 год: недооцінка
    /// дорожча за переоцінку, тому 1,1 год стає 1,25 год, а не 1,0 год.
    /// </summary>
    /// <param name="hours">Оцінка в годинах, більша за нуль.</param>
    /// <returns>Оцінка, кратна кроку.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Години не додатні або після округлення перевищують <see cref="MaxHours"/>.
    /// </exception>
    public static EffortEstimate FromHours(decimal hours)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hours);
        decimal roundedHours = Math.Ceiling(hours / StepHours) * StepHours;
        ArgumentOutOfRangeException.ThrowIfGreaterThan(roundedHours, MaxHours, nameof(hours));
        return new EffortEstimate(roundedHours);
    }

    /// <summary>Обчислює перевищення оцінки. Не змінює стан оцінки.</summary>
    /// <param name="spentHours">Фактично витрачені години, не від’ємні.</param>
    /// <returns>Години понад оцінку або 0, якщо оцінку не перевищено.</returns>
    public decimal OverrunHoursOf(decimal spentHours)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(spentHours);
        return Math.Max(spentHours - Hours, 0m);
    }

    /// <summary>Визначає стан виконання відносно оцінки. Не змінює стан оцінки.</summary>
    /// <param name="spentHours">Фактично витрачені години, не від’ємні.</param>
    /// <returns>Стан відносно оцінки.</returns>
    public EstimateStatus StatusOf(decimal spentHours)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(spentHours);
        if (spentHours > Hours)
        {
            return EstimateStatus.Overrun;
        }

        bool isNearLimit = spentHours >= Hours * NearLimitRatio;
        return isNearLimit ? EstimateStatus.NearLimit : EstimateStatus.WithinEstimate;
    }
}
