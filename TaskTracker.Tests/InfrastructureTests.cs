using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Storage;

namespace TaskTracker.Tests;

public class InfrastructureTests
{
    [Fact]
    public void StandardPolicy_TwoHours_ReturnsFullCost()
    {
        var policy = new StandardPricingPolicy();

        decimal price = policy.PriceOf(new WorkLog("DEV", 2m, 250m));

        Assert.Equal(500m, price);
    }

    [Fact]
    public void PartnerPolicy_TwoHours_ReturnsTenPercentOff()
    {
        var policy = new PartnerPricingPolicy();

        decimal price = policy.PriceOf(new WorkLog("DEV", 2m, 250m));

        Assert.Equal(450m, price);
    }

    [Fact]
    public void DiscountPolicy_RateAboveMax_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiscountPricingPolicy(0.6m));
    }

    [Fact]
    public void Repository_DuplicateId_Throws()
    {
        var repository = new InMemoryTaskRepository();
        repository.Add(NewTask(1));

        Assert.Throws<InvalidOperationException>(() => repository.Add(NewTask(1)));
    }

    [Fact]
    public void Repository_AddedTasks_AreReturnedByIdAndAll()
    {
        var repository = new InMemoryTaskRepository();
        repository.Add(NewTask(1));
        repository.Add(NewTask(2));

        IReadOnlyList<WorkTask> all = repository.GetAll();

        Assert.True(
            all.Count == 2 && repository.GetById(2) is not null && repository.GetById(9) is null);
    }

    [Fact]
    public void CsvReport_TaskWithLogs_HasHeaderAndRow()
    {
        WorkTask task = NewTask(1);
        task.Start();
        task.AddLog(new WorkLog("DEV", 2m, 100m));

        string csv = WorkTaskCsvReport.BuildCsv(new[] { task });

        Assert.True(csv.StartsWith("id;project;hours;status", StringComparison.Ordinal)
            && csv.Contains("1;CRM;2;InProgress", StringComparison.Ordinal));
    }

    [Fact]
    public void Result_OkAndFail_CarryValueOrError()
    {
        Result<int> ok = Result.Ok(5);
        Result<int> fail = Result.Fail<int>("помилка");

        Assert.True(ok.IsSuccess && ok.Value == 5 && !fail.IsSuccess && fail.Error == "помилка");
    }

    [Fact]
    public void DomainRuleException_StandardConstructors_UseDefaultRule()
    {
        var inner = new InvalidOperationException("причина");

        var empty = new DomainRuleException();
        var withMessage = new DomainRuleException("текст");
        var withInner = new DomainRuleException("текст", inner);

        Assert.True(empty.Rule == DomainRuleException.UnknownRule
            && withMessage.Message == "текст"
            && withInner.InnerException == inner);
    }

    private static WorkTask NewTask(int id) => new(id, "CRM", 10, new DateOnly(2026, 11, 1));
}
