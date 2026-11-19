namespace TaskTracker.Core.Legacy;

public class LegacyLog
{
    public string Activity { get; set; } = "";
    public decimal Hours { get; set; }
    public decimal HourlyRate { get; set; }
}

public class LegacyTask
{
    public int Id { get; set; }
    public int Status { get; set; }
    public decimal Total { get; set; }
    public string Email { get; set; } = "";
    public List<LegacyLog> Logs { get; set; } = new();

    public virtual void AddLog(LegacyLog log) => Logs.Add(log);
}

public class ArchivedTask : LegacyTask
{
    public ArchivedTask(int id) => Id = id;

    public override void AddLog(LegacyLog log)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
