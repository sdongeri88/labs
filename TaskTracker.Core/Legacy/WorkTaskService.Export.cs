using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

public sealed partial class WorkTaskService
{
    // БУЛО: при винятку файл лишається відкритим
    public void ExportReport(string path, WorkTask task)
    {
        var writer = new StreamWriter(path);
        writer.WriteLine($"Завдання {task.Id}");
        writer.WriteLine(CostOf(task)); // може кинути
        writer.Close();
    }
}
