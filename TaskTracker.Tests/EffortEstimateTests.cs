using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

public class EffortEstimateTests
{
    public static TheoryData<decimal, decimal> RoundingCases => new()
    {
        { 0.01m, 0.25m },
        { 0.25m, 0.25m },
        { 1.1m, 1.25m },
        { 7.5m, 7.5m },
        { 79.8m, 80m },
        { 80m, 80m },
    };

    public static TheoryData<decimal> InvalidHours => new() { 0m, -1m, 80.01m, 120m };

    public static TheoryData<decimal, EstimateStatus> StatusCases => new()
    {
        { 0m, EstimateStatus.WithinEstimate },
        { 7.99m, EstimateStatus.WithinEstimate },
        { 8m, EstimateStatus.NearLimit },
        { 10m, EstimateStatus.NearLimit },
        { 10.25m, EstimateStatus.Overrun },
    };

    [Theory]
    [MemberData(nameof(RoundingCases))]
    public void FromHours_AnyPositive_RoundsUpToQuarterHour(decimal hours, decimal expected)
    {
        EffortEstimate estimate = EffortEstimate.FromHours(hours);

        Assert.Equal(expected, estimate.Hours);
    }

    [Theory]
    [MemberData(nameof(InvalidHours))]
    public void FromHours_OutOfRange_ThrowsForHours(decimal hours)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => EffortEstimate.FromHours(hours));

        Assert.Equal("hours", ex.ParamName);
    }

    [Theory]
    [MemberData(nameof(StatusCases))]
    public void StatusOf_SpentHours_MatchesThresholds(decimal spentHours, EstimateStatus expected)
    {
        EffortEstimate estimate = EffortEstimate.FromHours(10m);

        Assert.Equal(expected, estimate.StatusOf(spentHours));
    }

    [Fact]
    public void OverrunHoursOf_SpentMoreThanEstimate_ReturnsDifference()
    {
        EffortEstimate estimate = EffortEstimate.FromHours(4m);

        Assert.Equal((0m, 1.5m), (estimate.OverrunHoursOf(3m), estimate.OverrunHoursOf(5.5m)));
    }
}
