namespace TaskTracker.Domain;

/// <summary>
/// Розрахунок завантаження виконавця: сума оцінок активних завдань,
/// поділена на тижневу місткість (розділ 1.4, таблиця 2). Без побічних ефектів.
/// </summary>
public static class WorkloadCalculator
{
    /// <summary>Нижня межа рівня «нормально», %.</summary>
    public const decimal NormalFromPercent = 50m;

    /// <summary>Верхня межа рівня «нормально», % (включно).</summary>
    public const decimal NormalToPercent = 100m;

    private const decimal PercentScale = 100m;

    /// <summary>Обчислює рядок звіту для одного виконавця.</summary>
    /// <param name="member">Виконавець.</param>
    /// <param name="board">Усі завдання дошки.</param>
    /// <returns>Завантаження виконавця.</returns>
    public static MemberWorkload Calculate(Member member, IEnumerable<BoardTask> board)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(board);

        List<BoardTask> active = board
            .Where(t => t.IsActive && t.AssigneeLogin == member.Login)
            .ToList();
        decimal hours = active.Sum(t => t.EstimateHours);
        decimal percent = LoadPercent(hours, member.WeeklyCapacityHours);
        return new MemberWorkload(
            member.Login,
            member.Name,
            active.Count,
            hours,
            member.WeeklyCapacityHours,
            percent,
            LevelOf(percent));
    }

    /// <summary>Завантаження у відсотках, округлене до цілого від нуля.</summary>
    /// <param name="hours">Години активних завдань; не від'ємні.</param>
    /// <param name="capacity">Місткість, годин; більша за нуль.</param>
    /// <returns>Відсоток завантаження.</returns>
    public static decimal LoadPercent(decimal hours, decimal capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(hours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        return Math.Round(hours * PercentScale / capacity, 0, MidpointRounding.AwayFromZero);
    }

    /// <summary>Визначає рівень завантаження за відсотком.</summary>
    /// <param name="percent">Відсоток завантаження.</param>
    /// <returns>Рівень за порогами 50 % і 100 %.</returns>
    public static WorkloadLevel LevelOf(decimal percent)
    {
        if (percent > NormalToPercent)
        {
            return WorkloadLevel.Overloaded;
        }

        return percent >= NormalFromPercent ? WorkloadLevel.Normal : WorkloadLevel.Free;
    }
}
