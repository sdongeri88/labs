namespace TaskTracker.Domain;

/// <summary>
/// Таблиця дозволених переходів між колонками дошки (розділ 1.5 специфікації).
/// Перехід, якого немає в таблиці, заборонений.
/// </summary>
public static class TransitionTable
{
    private static Dictionary<BoardColumn, BoardColumn[]> Allowed { get; } =
        new Dictionary<BoardColumn, BoardColumn[]>
        {
            [BoardColumn.Backlog] = [BoardColumn.Todo],
            [BoardColumn.Todo] = [BoardColumn.Backlog, BoardColumn.InProgress],
            [BoardColumn.InProgress] = [BoardColumn.Todo, BoardColumn.Review],
            [BoardColumn.Review] = [BoardColumn.InProgress, BoardColumn.Done],
            [BoardColumn.Done] = [],
        };

    /// <summary>Перевіряє, чи дозволяє таблиця перехід.</summary>
    /// <param name="from">Поточна колонка.</param>
    /// <param name="to">Цільова колонка.</param>
    /// <returns>true, якщо перехід є в таблиці.</returns>
    public static bool IsAllowed(BoardColumn from, BoardColumn to) =>
        Allowed.TryGetValue(from, out BoardColumn[]? targets) && Array.IndexOf(targets, to) >= 0;

    /// <summary>Повертає колонки, до яких можна перейти з поточної.</summary>
    /// <param name="from">Поточна колонка.</param>
    /// <returns>Дозволені цільові колонки (порожньо для Done).</returns>
    public static IReadOnlyList<BoardColumn> TargetsFrom(BoardColumn from) =>
        Allowed.TryGetValue(from, out BoardColumn[]? targets) ? targets : [];
}
