using System.Text.Json;

namespace TaskTracker.Core.Legacy;

// Конкретні «важкі» залежності, які TaskManager створює сам через new.
public class FileTaskStore
{
    public void SaveToFile(LegacyTask task, string path)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(task));
    }
}

public class SmtpMailer
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine("SMTP → " + to + ": " + subject + " / " + body);
    }
}
