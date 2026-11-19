using TaskTracker.Core.Requests;
using Xunit;

namespace TaskTracker.Tests;

public class RequestTotalCalculatorTests
{
    [Fact]
    public void Calculate_RegularClient_Returns1292()
    {
        TotalRequest request = new(
            RequestId: 1,
            Client: new ClientInfo("Іваненко", "i@ex.com", true),
            Lines: new List<StageLine>
            {
                new() { StageCode = "ST-1", Hours = 12, HourlyRate = 100m },
                new() { StageCode = "ST-2", Hours = 1, HourlyRate = 250m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: RequestStatus.New,
            TravelFee: 60m);

        TotalResult result = RequestTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(RequestStatus.Approved, result.Status);
    }

    [Fact]
    public void Calculate_SmallRequest_AddsTravelFee()
    {
        TotalRequest request = new(
            RequestId: 2,
            Client: new ClientInfo("Петренко", "p@ex.com", false),
            Lines: new List<StageLine>
            {
                new() { StageCode = "ST-1", Hours = 2, HourlyRate = 200m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: PricingRules.SaleCoupon,
            Status: RequestStatus.Approved,
            TravelFee: 60m);

        TotalResult result = RequestTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(RequestStatus.Completed, result.Status);
    }

    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            RequestGuards.EnsureLinesValid(new List<StageLine>()));
    }

    [Fact]
    public void Calculate_ZeroHours_ThrowsRange()
    {
        StageLine line = new() { StageCode = "ST-1", Hours = 0, HourlyRate = 10m };
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RequestGuards.EnsureLineValid(line));
    }
}
