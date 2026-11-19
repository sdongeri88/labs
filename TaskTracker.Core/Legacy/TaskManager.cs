namespace TaskTracker.Core.Legacy;

public class TaskManager
{
    private readonly FileTaskStore _store = new FileTaskStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Register(LegacyTask task, string clientType)
    {
        decimal total = 0m;
        foreach (var log in task.Logs)
            total += log.HourlyRate * log.Hours;

        if (clientType == "regular")
            total = total * 0.95m;
        else if (clientType == "partner")
            total = total * 0.90m;
        else if (clientType == "internal")
            total = total * 0.70m;

        if (total > 1000m)
            total = total - 50m;

        task.Total = total;
        task.Status = 1;
        _store.SaveToFile(task, "tasks.json");
        _mailer.Send(task.Email, "Task " + task.Id, "" + total);
        return total;
    }

    public string BuildCsvReport(List<LegacyTask> tasks)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var t in tasks)
            text += t.Id + ";" + t.Total + ";" + t.Status;

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyTask task)
    {
        LegacyTask archived = new ArchivedTask(task.Id);
        foreach (var log in task.Logs)
            archived.AddLog(log);

        _store.SaveToFile(archived, "archive.json");
    }
}
