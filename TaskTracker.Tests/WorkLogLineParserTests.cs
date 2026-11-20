using TaskTracker.Core.Domain;
using TaskTracker.Core.Parsing;

namespace TaskTracker.Tests;

public class WorkLogLineParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("DEV;2")]
    [InlineData(";2;250")]
    [InlineData("ДУЖЕ-ДОВГА-НАЗВА-ВИДУ-РОБОТИ;2;250")]
    [InlineData("DEV;0;250")]
    [InlineData("DEV;25;250")]
    [InlineData("DEV;-2;250")]
    [InlineData("DEV;1,5;250")]
    [InlineData("DEV;2;-10.50")]
    [InlineData("DEV;2;10000.01")]
    public void TryParse_InvalidInput_ReturnsFalse(string raw)
    {
        bool ok = WorkLogLineParser.TryParse(raw, out WorkLog? log);

        Assert.False(ok);
        Assert.Null(log);
    }

    [Fact]
    public void TryParse_ValidLine_ReturnsTrimmedLog()
    {
        bool ok = WorkLogLineParser.TryParse(" REVIEW ;1.5;300", out WorkLog? log);

        Assert.True(ok);
        Assert.Equal(("REVIEW", 1.5m, 300m), (log!.Activity, log.Hours, log.HourlyRate));
    }
}
