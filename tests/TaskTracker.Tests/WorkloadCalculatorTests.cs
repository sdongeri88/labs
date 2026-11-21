using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Розрахунок завантаження: таблиця 2 розділу 1.4, округлення від нуля.</summary>
public class WorkloadCalculatorTests
{
    [Theory]
    [InlineData(0, 40, 0)]
    [InlineData(10, 40, 25)]
    [InlineData(20, 40, 50)]
    [InlineData(40, 40, 100)]
    [InlineData(41, 40, 103)]
    [InlineData(1, 3, 33)]
    [InlineData(2, 3, 67)]
    public void LoadPercent_HoursAndCapacity_RoundsAwayFromZero(
        int hours, int capacity, int expected)
    {
        decimal actual = WorkloadCalculator.LoadPercent(hours, capacity);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, WorkloadLevel.Free)]
    [InlineData(49, WorkloadLevel.Free)]
    [InlineData(50, WorkloadLevel.Normal)]
    [InlineData(100, WorkloadLevel.Normal)]
    [InlineData(101, WorkloadLevel.Overloaded)]
    public void LevelOf_Percent_ReturnsLevel(int percent, WorkloadLevel expected)
    {
        Assert.Equal(expected, WorkloadCalculator.LevelOf(percent));
    }

    [Fact]
    public void LoadPercent_ZeroCapacity_ThrowsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => WorkloadCalculator.LoadPercent(1m, 0m));
    }

    [Fact]
    public void Calculate_OnlyActiveTasksOfMember_AreCounted()
    {
        var member = new Member("Ivan", "Іван Мороз", 20m);
        BoardTask[] board =
        [
            Tasks.In(BoardColumn.InProgress, "ivan", 6m),
            Tasks.In(BoardColumn.Review, "ivan", 5m),
            Tasks.In(BoardColumn.Done, "ivan", 30m),
            Tasks.In(BoardColumn.Todo, "ivan", 30m),
            Tasks.In(BoardColumn.InProgress, "olena", 30m),
        ];

        MemberWorkload row = WorkloadCalculator.Calculate(member, board);

        Assert.Equal(
            (2, 11m, 55m, WorkloadLevel.Normal),
            (row.ActiveTasks, row.ActiveHours, row.LoadPercent, row.Level));
    }
}
