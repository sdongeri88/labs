namespace TaskTracker.Domain;

/// <summary>
/// Профільне правило варіанта: таблиця переходів, обов'язковий виконавець
/// для роботи, ліміт WIP колонки й особистий ліміт виконавця (розділ 1.4).
/// </summary>
public sealed class WipMovePolicy : IMovePolicy
{
    private readonly WipLimits _limits;

    /// <summary>Створює правило з заданими лімітами.</summary>
    /// <param name="limits">Ліміти WIP.</param>
    public WipMovePolicy(WipLimits limits)
    {
        _limits = limits ?? throw new ArgumentNullException(nameof(limits));
    }

    /// <inheritdoc/>
    public MoveVerdict Check(
        BoardTask task, BoardColumn target, IReadOnlyCollection<BoardTask> board)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(board);

        if (!TransitionTable.IsAllowed(task.Column, target))
        {
            return MoveVerdict.ForbiddenTransition;
        }

        if (target == BoardColumn.InProgress && task.AssigneeLogin is null)
        {
            return MoveVerdict.AssigneeRequired;
        }

        if (IsColumnFull(target, board))
        {
            return MoveVerdict.ColumnLimitReached;
        }

        return BreaksPersonalLimit(task, target, board)
            ? MoveVerdict.PersonalLimitReached
            : MoveVerdict.Allowed;
    }

    private bool IsColumnFull(BoardColumn target, IReadOnlyCollection<BoardTask> board)
    {
        int? limit = _limits.ForColumn(target);
        return limit is not null && board.Count(t => t.Column == target) >= limit;
    }

    private bool BreaksPersonalLimit(
        BoardTask task, BoardColumn target, IReadOnlyCollection<BoardTask> board)
    {
        bool entersWork = target == BoardColumn.InProgress && !task.IsActive;
        if (!entersWork)
        {
            return false;
        }

        int active = board.Count(t => t.IsActive && t.AssigneeLogin == task.AssigneeLogin);
        return active >= _limits.Personal;
    }
}
