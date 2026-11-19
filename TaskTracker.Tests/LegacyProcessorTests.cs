using TaskTracker.Core.Legacy;
using Xunit;

namespace TaskTracker.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<StageLine> lines = new()
        {
            new StageLine { StageCode = "ST-1", Hours = 12, HourlyRate = 100m },
            new StageLine { StageCode = "ST-2", Hours = 1, HourlyRate = 250m },
        };
        decimal total = LegacyProcessor.Process(
            1, "Іваненко", "i@ex.com", true, lines,
            new DateOnly(2026, 3, 10), "", "New", false,
            60m, out string next);
        Assert.Equal(1292.70m, total);
        Assert.Equal("Approved", next);
    }

    [Fact]
    public void Process_SmallRequest_AddsTravelFee()
    {
        List<StageLine> lines = new()
        {
            new StageLine { StageCode = "ST-1", Hours = 2, HourlyRate = 200m },
        };
        decimal total = LegacyProcessor.Process(
            2, "Петренко", "p@ex.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", "Approved",
            false, 60m, out string next);
        Assert.Equal(420m, total);
        Assert.Equal("Completed", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3, "Коваль", "k@ex.com", false,
            new List<StageLine>(),
            new DateOnly(2026, 3, 10), "", "New", false,
            60m, out string next);
        Assert.Equal(-1m, total);
    }
}
