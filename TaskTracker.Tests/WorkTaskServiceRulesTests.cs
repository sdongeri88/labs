using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Services;

namespace TaskTracker.Tests;

public class WorkTaskServiceRulesTests
{
    [Fact]
    public void Constructor_NullRepository_ThrowsForRepository()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new WorkTaskService(
            null!,
            new StubPricingPolicy(1m),
            new RecordingNotifier(),
            NullLogger<WorkTaskService>.Instance));

        Assert.Equal("repository", ex.ParamName);
    }

    [Fact]
    public void AddLog_TaskNotStarted_ThrowsEditableRule()
    {
        var repository = new FakeTaskRepository();
        repository.Add(NewTask(1));

        var ex = Assert.Throws<DomainRuleException>(
            () => Service(repository).AddLog(1, new WorkLog("DEV", 1m, 100m)));

        Assert.Equal("task.editable", ex.Rule);
    }

    [Fact]
    public void AddLog_LongEntry_IsStillAdded()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = NewTask(2);
        task.Start();
        repository.Add(task);

        Service(repository).AddLog(2, new WorkLog("DEV", 13m, 100m));

        Assert.Equal(13m, task.TotalHours);
    }

    [Fact]
    public void AddLog_UnknownTask_ThrowsExistsRule()
    {
        var ex = Assert.Throws<DomainRuleException>(
            () => Service(new FakeTaskRepository()).AddLog(9, new WorkLog("DEV", 1m, 100m)));

        Assert.Equal("task.exists", ex.Rule);
    }

    [Fact]
    public void CloseTask_TaskInProgress_ThrowsClosableRule()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = NewTask(3);
        task.Start();
        repository.Add(task);

        var ex = Assert.Throws<DomainRuleException>(() => Service(repository).CloseTask(3, 1000m));

        Assert.Equal("task.closable", ex.Rule);
    }

    [Fact]
    public void Cancel_NewTask_IsCancelled()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = NewTask(4);
        repository.Add(task);

        Service(repository).Cancel(4, "замовник відмовився");

        Assert.Equal(WorkTaskStatus.Cancelled, task.Status);
    }

    [Fact]
    public void Cancel_DoneTask_WrapsInvalidOperation()
    {
        var repository = new FakeTaskRepository();
        WorkTask task = NewTask(5);
        task.Start();
        task.AddLog(new WorkLog("DEV", 1m, 100m));
        task.SubmitForReview();
        task.Complete();
        repository.Add(task);

        var ex = Assert.Throws<DomainRuleException>(() => Service(repository).Cancel(5, "пізно"));

        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    private static WorkTask NewTask(int id) => new(id, "CRM", 10, new DateOnly(2026, 11, 1));

    private static WorkTaskService Service(FakeTaskRepository repository) =>
        new(repository, new StubPricingPolicy(100m), new RecordingNotifier(),
            NullLogger<WorkTaskService>.Instance);
}
