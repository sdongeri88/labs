using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

public sealed partial class WorkTaskService
{
    // БУЛО: барикади немає, ядро розбирає рядки саме
    public void AddLog(int taskId, string activity, string hours, string rate)
    {
        var task = _repository.GetById(taskId);
        var h = decimal.Parse(hours);
        var r = decimal.Parse(rate);
        task.AddLog(new WorkLog(activity, h, r));
    }
}
