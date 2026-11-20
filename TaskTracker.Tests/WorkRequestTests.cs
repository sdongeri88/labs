using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

public class WorkRequestTests
{
    [Fact]
    public void CalculateCost_RegularClientOver1000_AppliesTenPercentAndVat()
    {
        WorkRequest request = Request(("DEV", 2, 300m), ("QA", 3, 200m));

        decimal cost = request.CalculateCost(true);

        Assert.Equal(1296.00m, cost);
    }

    [Fact]
    public void CalculateCost_LargeRequest_AppliesFifteenPercentAndVat()
    {
        WorkRequest request = Request(("DEV", 30, 200m));

        decimal cost = request.CalculateCost(false);

        Assert.Equal(6120.00m, cost);
    }

    [Fact]
    public void CalculateCost_MoreThanTenStages_SubtractsBulkDiscount()
    {
        WorkRequest request = Stages(11, 10m);

        decimal cost = request.CalculateCost(false);

        Assert.Equal(12.00m, cost);
    }

    [Fact]
    public void CalculateCost_BulkDiscountAboveSum_NeverNegative()
    {
        WorkRequest request = Stages(11, 5m);

        decimal cost = request.CalculateCost(false);

        Assert.Equal(0m, cost);
    }

    [Fact]
    public void TryChangeStatus_AllowedChain_ReachesDone()
    {
        WorkRequest request = Request(("DEV", 1, 100m));

        bool started = request.TryChangeStatus(WorkRequest.StatusInProgress);
        bool done = request.TryChangeStatus(WorkRequest.StatusDone);

        Assert.True(started && done && request.Status == WorkRequest.StatusDone);
    }

    [Fact]
    public void TryChangeStatus_NewToCancelled_Succeeds()
    {
        WorkRequest request = Request(("DEV", 1, 100m));

        bool changed = request.TryChangeStatus(WorkRequest.StatusCancelled);

        Assert.True(changed);
    }

    [Fact]
    public void TryChangeStatus_NewToDone_IsRejected()
    {
        WorkRequest request = Request(("DEV", 1, 100m));

        bool changed = request.TryChangeStatus(WorkRequest.StatusDone);

        Assert.False(changed);
    }

    [Theory]
    [InlineData("R-1", "Іван", 1, true)]
    [InlineData("R-1", "Ян", 1, false)]
    [InlineData("R-1", "Іван", 0, false)]
    [InlineData("", "Іван", 1, false)]
    public void IsValid_ByData_MatchesRules(string id, string client, int stages, bool expected)
    {
        var request = new WorkRequest(id, client);
        for (int i = 0; i < stages; i++)
        {
            request.AddStage("DEV", 1, 100m);
        }

        bool actual = request.IsValid();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void BuildReport_OneStage_ListsStageAndTotal()
    {
        WorkRequest request = Request(("DEV", 2, 300m));

        string report = request.BuildReport();

        Assert.True(report.StartsWith("Етап: DEV; годин: 2; ставка: 300", StringComparison.Ordinal)
            && report.Contains("Разом: ", StringComparison.Ordinal));
    }

    [Fact]
    public void FindById_ExistingAndMissing_ReturnsRequestOrNull()
    {
        var list = new List<WorkRequest> { new("R-1", "Іван"), new("R-2", "Олена") };

        WorkRequest? found = WorkRequest.FindById(list, "R-2");
        WorkRequest? missing = WorkRequest.FindById(list, "R-9");

        Assert.True(found?.ClientName == "Олена" && missing is null);
    }

    private static WorkRequest Request(params (string Code, int Hours, decimal Rate)[] stages)
    {
        var request = new WorkRequest("R-1", "Іван");
        foreach ((string code, int hours, decimal rate) in stages)
        {
            request.AddStage(code, hours, rate);
        }

        return request;
    }

    private static WorkRequest Stages(int count, decimal rate)
    {
        var request = new WorkRequest("R-2", "Олена");
        for (int i = 0; i < count; i++)
        {
            request.AddStage("ST-" + i, 1, rate);
        }

        return request;
    }
}
