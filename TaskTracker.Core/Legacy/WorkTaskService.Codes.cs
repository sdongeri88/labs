using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

public sealed partial class WorkTaskService
{
    // БУЛО: коди повернення без назв і сенсу
    public int CloseTask(int taskId, decimal approvedBudget)
    {
        var task = _repository.GetById(taskId);
        if (task == null) return -1;
        if (task.Status != WorkTaskStatus.Review) return -2;
        if (approvedBudget < CostOf(task)) return -3;
        task.Complete();
        return 0;
    }
}
