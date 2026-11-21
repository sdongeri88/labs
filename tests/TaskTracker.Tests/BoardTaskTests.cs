using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Життєвий цикл документа: таблиця переходів розділу 1.5.</summary>
public class BoardTaskTests
{
    private readonly WipMovePolicy _policy = new(WipLimits.Default);

    [Theory]
    [InlineData(BoardColumn.Backlog, BoardColumn.Todo)]
    [InlineData(BoardColumn.Todo, BoardColumn.Backlog)]
    [InlineData(BoardColumn.Todo, BoardColumn.InProgress)]
    [InlineData(BoardColumn.InProgress, BoardColumn.Todo)]
    [InlineData(BoardColumn.InProgress, BoardColumn.Review)]
    [InlineData(BoardColumn.Review, BoardColumn.InProgress)]
    [InlineData(BoardColumn.Review, BoardColumn.Done)]
    public void MoveTo_AllowedTransition_ChangesColumnAndWritesHistory(
        BoardColumn from, BoardColumn to)
    {
        BoardTask task = Tasks.In(from, "ivan");

        task.MoveTo(to, _policy, [task], FixedClock.Moment);

        Assert.Equal(to, task.Column);
        Assert.Equal(new TaskMove(from, to, FixedClock.Moment), Assert.Single(task.History));
    }

    [Theory]
    [InlineData(BoardColumn.Backlog, BoardColumn.InProgress)]
    [InlineData(BoardColumn.Backlog, BoardColumn.Done)]
    [InlineData(BoardColumn.Todo, BoardColumn.Done)]
    [InlineData(BoardColumn.Done, BoardColumn.Review)]
    [InlineData(BoardColumn.Done, BoardColumn.Backlog)]
    public void MoveTo_ForbiddenTransition_ThrowsAndKeepsColumn(BoardColumn from, BoardColumn to)
    {
        BoardTask task = Tasks.In(from, "ivan");

        var ex = Assert.Throws<MoveRejectedException>(
            () => task.MoveTo(to, _policy, [task], FixedClock.Moment));

        Assert.Equal(MoveVerdict.ForbiddenTransition, ex.Verdict);
        Assert.Equal(from, task.Column);
        Assert.Empty(task.History);
    }

    [Fact]
    public void MoveTo_NoAssigneeIntoWork_ExplainsAssigneeRequired()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo);

        var ex = Assert.Throws<MoveRejectedException>(
            () => task.MoveTo(BoardColumn.InProgress, _policy, [task], FixedClock.Moment));

        Assert.Equal(MoveVerdict.AssigneeRequired, ex.Verdict);
        Assert.Contains("без виконавця", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void MoveTo_AssigneeAtPersonalLimit_NamesAssigneeInMessage()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo, "ivan");
        List<BoardTask> board = [task, .. Tasks.Many(2, BoardColumn.Review, "ivan")];

        var ex = Assert.Throws<MoveRejectedException>(
            () => task.MoveTo(BoardColumn.InProgress, _policy, board, FixedClock.Moment));

        Assert.Equal("У ivan уже максимум активних завдань.", ex.Message);
    }

    [Fact]
    public void TargetsFrom_Done_IsEmpty()
    {
        Assert.Empty(TransitionTable.TargetsFrom(BoardColumn.Done));
    }

    [Fact]
    public void AssignTo_DoneTask_ThrowsDomainException()
    {
        BoardTask task = Tasks.In(BoardColumn.Done, "ivan");

        Assert.Throws<DomainException>(() => task.AssignTo("olena"));
    }

    [Fact]
    public void Constructor_NegativeEstimate_ThrowsOutOfRange()
    {
        var details = new TaskDetails("Звіт", TaskPriority.Low, -1m);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BoardTask(1, details, new DateOnly(2026, 11, 1)));
    }
}
