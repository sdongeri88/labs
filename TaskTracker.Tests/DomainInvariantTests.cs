using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

namespace TaskTracker.Tests;

public class DomainInvariantTests
{
    [Fact]
    public void WorkLog_WithZeroHours_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WorkLog("DEV", 0m, 100m));
    }

    [Fact]
    public void WorkTask_CompleteWhenNew_ThrowsInvalidOperation()
    {
        WorkTask task = NewTask(1);

        Assert.Throws<InvalidOperationException>(() => task.Complete());
    }

    [Fact]
    public void CostOf_WithTenPercentDiscount_Returns180()
    {
        var service = new WorkTaskService(
            new InMemoryTaskRepository(),
            new DiscountPricingPolicy(0.10m),
            new SilentNotifier(),
            NullLogger<WorkTaskService>.Instance);
        WorkTask task = NewTask(2);
        task.Start();
        task.AddLog(new WorkLog("DEV", 2m, 100m));

        Assert.Equal(180m, service.CostOf(task));
    }

    private static WorkTask NewTask(int id) =>
        new(id, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));

    private sealed class SilentNotifier : INotifier
    {
        public void Notify(string recipient, string message)
        {
        }
    }
}
