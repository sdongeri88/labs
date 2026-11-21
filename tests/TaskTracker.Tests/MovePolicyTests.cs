using TaskTracker.Domain;
using C = TaskTracker.Domain.BoardColumn;
using V = TaskTracker.Domain.MoveVerdict;

namespace TaskTracker.Tests;

/// <summary>Профільне правило: таблиця рішень розділу 1.4 специфікації, рядок = випадок.</summary>
public class MovePolicyTests
{
    private readonly WipMovePolicy _policy = new(WipLimits.Default);

    [Theory]
    [InlineData(C.Backlog, C.Todo, false, 7, 0, V.Allowed)]
    [InlineData(C.Backlog, C.Todo, false, 8, 0, V.ColumnLimitReached)]
    [InlineData(C.Todo, C.InProgress, false, 0, 0, V.AssigneeRequired)]
    [InlineData(C.Todo, C.InProgress, true, 4, 1, V.Allowed)]
    [InlineData(C.Todo, C.InProgress, true, 5, 0, V.ColumnLimitReached)]
    [InlineData(C.Todo, C.InProgress, true, 3, 2, V.PersonalLimitReached)]
    [InlineData(C.InProgress, C.Review, true, 2, 1, V.Allowed)]
    [InlineData(C.InProgress, C.Review, true, 3, 1, V.ColumnLimitReached)]
    [InlineData(C.Review, C.InProgress, true, 0, 1, V.Allowed)]
    [InlineData(C.Review, C.Done, true, 20, 1, V.Allowed)]
    [InlineData(C.Backlog, C.Done, true, 0, 0, V.ForbiddenTransition)]
    [InlineData(C.Done, C.Todo, true, 0, 0, V.ForbiddenTransition)]
    public void Check_DecisionTableRow_ReturnsVerdict(
        BoardColumn from,
        BoardColumn to,
        bool hasAssignee,
        int alreadyInTarget,
        int otherActiveOfAssignee,
        MoveVerdict expected)
    {
        BoardTask task = Tasks.In(from, hasAssignee ? "ivan" : null);
        List<BoardTask> board = [task];
        board.AddRange(Tasks.Many(alreadyInTarget, to, "other"));
        BoardColumn activeColumn =
            to == BoardColumn.Review ? BoardColumn.InProgress : BoardColumn.Review;
        board.AddRange(Tasks.Many(otherActiveOfAssignee, activeColumn, "ivan"));

        MoveVerdict actual = _policy.Check(task, to, board);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Check_AssigneeAtPersonalLimitMovesWithinActive_IsAllowed()
    {
        BoardTask task = Tasks.In(BoardColumn.InProgress, "ivan");
        List<BoardTask> board = [task, Tasks.In(BoardColumn.InProgress, "ivan")];

        MoveVerdict actual = _policy.Check(task, BoardColumn.Review, board);

        Assert.Equal(MoveVerdict.Allowed, actual);
    }

    [Fact]
    public void Check_NullBoard_ThrowsArgumentNull()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo, "ivan");

        Assert.Throws<ArgumentNullException>(
            () => _policy.Check(task, BoardColumn.InProgress, null!));
    }

    [Theory]
    [InlineData(C.Todo, 8)]
    [InlineData(C.InProgress, 5)]
    [InlineData(C.Review, 3)]
    public void ForColumn_LimitedColumn_ReturnsAgreedLimit(BoardColumn column, int expected)
    {
        Assert.Equal(expected, WipLimits.Default.ForColumn(column));
    }

    [Fact]
    public void Default_PersonalLimit_IsTwoActiveTasks()
    {
        Assert.Equal(2, WipLimits.Default.Personal);
    }

    [Fact]
    public void Check_AssigneeWithTwoActiveInReview_CannotStartThird()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo, "ivan");
        List<BoardTask> board = [task, .. Tasks.Many(2, BoardColumn.Review, "ivan")];

        MoveVerdict actual = _policy.Check(task, BoardColumn.InProgress, board);

        Assert.Equal(MoveVerdict.PersonalLimitReached, actual);
    }

    [Theory]
    [InlineData(C.Backlog)]
    [InlineData(C.Done)]
    public void ForColumn_UnlimitedColumn_ReturnsNull(BoardColumn column)
    {
        Assert.Null(WipLimits.Default.ForColumn(column));
    }

    [Fact]
    public void WipLimits_ZeroLimit_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new WipLimits(0, 1, 1, 1));
    }
}
