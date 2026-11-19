namespace TaskTracker.Core.Legacy;

public sealed class StageLine
{
    public string StageCode { get; set; } = "";
    public int Hours { get; set; }
    public decimal HourlyRate { get; set; }
}
