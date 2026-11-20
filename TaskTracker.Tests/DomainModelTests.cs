using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

public class DomainModelTests
{
    [Fact]
    public void Member_EmailWithoutAt_ThrowsForEmail()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Member(1, "Іван", "ivan.mail"));

        Assert.Equal("email", ex.ParamName);
    }

    [Fact]
    public void Member_ValidData_KeepsValuesAndLeadFlag()
    {
        var member = new Member(3, "Олена", "olena@team.ua") { IsLead = true };

        Assert.Equal(
            (3, "Олена", "olena@team.ua", true),
            (member.Id, member.Name, member.Email, member.IsLead));
    }

    [Fact]
    public void Project_OpenThenClose_ReturnsToZero()
    {
        var project = new Project("CRM", "Облік клієнтів", 250m);
        project.OpenTask();

        project.CloseTask();

        Assert.Equal(0, project.OpenTasks);
    }

    [Fact]
    public void Project_CloseWithoutOpenTasks_Throws()
    {
        var project = new Project("CRM", "Облік клієнтів", 250m);

        Assert.Throws<InvalidOperationException>(() => project.CloseTask());
    }

    [Fact]
    public void Project_ZeroRate_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Project("CRM", "Облік", 0m));
    }

    [Fact]
    public void ArchivedWorkTask_DoneTask_ExposesTaskData()
    {
        WorkTask task = DoneTask();
        var archivedAt = new DateOnly(2026, 11, 20);

        var archived = new ArchivedWorkTask(task, archivedAt);

        Assert.Equal(
            (1, WorkTaskStatus.Done, 1, archivedAt),
            (archived.Id, archived.Status, archived.Logs.Count, archived.ArchivedAt));
    }

    [Fact]
    public void ArchivedWorkTask_NewTask_ThrowsForTask()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => new ArchivedWorkTask(NewTask(), DateOnly.MinValue));

        Assert.Equal("task", ex.ParamName);
    }

    [Fact]
    public void WorkTask_ReturnToWorkFromReview_IsInProgress()
    {
        WorkTask task = NewTask();
        task.Start();
        task.AddLog(new WorkLog("DEV", 1m, 100m));
        task.SubmitForReview();

        task.ReturnToWork();

        Assert.Equal(WorkTaskStatus.InProgress, task.Status);
    }

    [Fact]
    public void WorkTask_SubmitWithoutLogs_Throws()
    {
        WorkTask task = NewTask();
        task.Start();

        Assert.Throws<InvalidOperationException>(() => task.SubmitForReview());
    }

    [Fact]
    public void WorkTask_CancelNew_IsCancelled()
    {
        WorkTask task = NewTask();

        task.Cancel();

        Assert.Equal(WorkTaskStatus.Cancelled, task.Status);
    }

    [Fact]
    public void WorkTask_CancelDone_Throws()
    {
        WorkTask task = DoneTask();

        Assert.Throws<InvalidOperationException>(() => task.Cancel());
    }

    private static WorkTask NewTask() => new(1, "CRM", 10, new DateOnly(2026, 11, 1));

    private static WorkTask DoneTask()
    {
        WorkTask task = NewTask();
        task.Start();
        task.AddLog(new WorkLog("DEV", 2m, 100m));
        task.SubmitForReview();
        task.Complete();
        return task;
    }
}
