using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Services;

namespace TaskTracker.Tests;

public class WorkTaskServiceTests
{
    [Fact]
    public void CostOf_TwoLogs_SumsPricesFromPolicy()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = NewTask(7);
        task.Start();
        task.AddLog(new WorkLog("DEV", 2m, 100m));
        task.AddLog(new WorkLog("REVIEW", 1m, 300m));
        WorkTaskService service = NewService(repository, new StubPricingPolicy(25m));

        decimal actual = service.CostOf(task);

        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Register_NewTask_StoresAndNotifiesLeadOnce()
    {
        var repository = new FakeTaskRepository();
        var notifier = new RecordingNotifier();
        WorkTaskService service = NewService(repository, new StubPricingPolicy(1m), notifier);

        service.Register(NewTask(1));

        Assert.NotNull(repository.GetById(1));
        Assert.Single(notifier.Messages);
    }

    [Fact]
    public void Load_UnknownId_ThrowsTaskExistsRule()
    {
        WorkTaskService service = NewService(new FakeTaskRepository(), new StubPricingPolicy(1m));

        var ex = Assert.Throws<DomainRuleException>(() => service.Load(42));

        Assert.Equal("task.exists", ex.Rule);
    }

    [Fact]
    public void AddLog_TaskInProgress_AddsLogToStoredTask()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = NewTask(3);
        task.Start();
        repository.Add(task);
        WorkTaskService service = NewService(repository, new StubPricingPolicy(1m));

        service.AddLog(3, new WorkLog("DEV", 2m, 250m));

        Assert.Equal(2m, repository.GetById(3)!.TotalHours);
    }

    [Fact]
    public void CloseTask_ReviewWithinBudget_CompletesTask()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = InReview(5);
        repository.Add(task);
        WorkTaskService service = NewService(repository, new StubPricingPolicy(100m));

        service.CloseTask(5, 1000m);

        Assert.Equal(WorkTaskStatus.Done, task.Status);
    }

    [Fact]
    public void CloseTask_CostOverBudget_ThrowsBudgetRule()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = InReview(6);
        repository.Add(task);
        WorkTaskService service = NewService(repository, new StubPricingPolicy(800m));

        var ex = Assert.Throws<DomainRuleException>(() => service.CloseTask(6, 500m));

        Assert.Equal("task.budget", ex.Rule);
        Assert.Equal(WorkTaskStatus.Review, task.Status);
    }

    private static WorkTask NewTask(int id) =>
        new(id, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));

    private static WorkTask InReview(int id)
    {
        WorkTask task = NewTask(id);
        task.Start();
        task.AddLog(new WorkLog("DEV", 1m, 100m));
        task.SubmitForReview();
        return task;
    }

    private static WorkTaskService NewService(
        FakeTaskRepository repository,
        StubPricingPolicy pricing,
        RecordingNotifier? notifier = null) =>
        new(repository, pricing, notifier ?? new RecordingNotifier(),
            NullLogger<WorkTaskService>.Instance);
}
