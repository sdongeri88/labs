using Microsoft.Extensions.Logging;
using TaskTracker.Application.Services;
using TaskTracker.Application.Validation;
using TaskTracker.Domain;
using TaskTracker.Infrastructure.Seed;

namespace TaskTracker.Tests;

/// <summary>Сценарій-ядро UC-02 «Перемістити завдання дошкою».</summary>
public class MoveServiceTests
{
    [Fact]
    public void Move_AllowedTransition_MovesAndRecordsFixedTime()
    {
        BoardTask task = Tasks.In(BoardColumn.Todo, "ivan");
        MoveService service = NewService(new FakeTaskRepository(task));

        Result<BoardTask> result = service.Move(Tasks.IdOf(task), "inprogress");

        Assert.True(result.IsSuccess);
        Assert.Equal(BoardColumn.InProgress, task.Column);
        Assert.Equal(FixedClock.Moment, Assert.Single(task.History).At);
    }

    [Theory]
    [InlineData("12345", "Todo")]
    [InlineData("abc", "Todo")]
    [InlineData("1", "Нікуди")]
    public void Move_BadInput_FailsWithoutException(string id, string column)
    {
        MoveService service = NewService(new FakeTaskRepository(Tasks.In(BoardColumn.Todo)));

        Assert.False(service.Move(id, column).IsSuccess);
    }

    [Fact]
    public void Move_RuleRejects_ThrowsAndLogsWarning()
    {
        BoardTask task = Tasks.In(BoardColumn.Backlog);
        var log = new SpyLogger<MoveService>();
        MoveService service = NewService(new FakeTaskRepository(task), log);

        var ex = Assert.Throws<MoveRejectedException>(
            () => service.Move(Tasks.IdOf(task), "Done"));

        Assert.Equal(MoveVerdict.ForbiddenTransition, ex.Verdict);
        Assert.Contains(LogLevel.Warning, log.Levels);
    }

    [Fact]
    public void Move_IntoFullReviewOfDemoBoard_RejectedByColumnLimit()
    {
        DemoBoard demo = DemoBoardFactory.Create(new DateOnly(2026, 11, 20));
        BoardTask inWork = demo.Tasks.First(t => t.Column == BoardColumn.InProgress);
        MoveService service = NewService(new FakeTaskRepository(demo.Tasks.ToArray()));

        var ex = Assert.Throws<MoveRejectedException>(
            () => service.Move(Tasks.IdOf(inWork), "Review"));

        Assert.Equal(MoveVerdict.ColumnLimitReached, ex.Verdict);
    }

    private static MoveService NewService(
        FakeTaskRepository repository, SpyLogger<MoveService>? log = null) =>
        new(
            repository,
            new WipMovePolicy(WipLimits.Default),
            new FixedClock(),
            log ?? new SpyLogger<MoveService>());
}
