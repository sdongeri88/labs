namespace TaskTracker.Legacy;

public sealed class TimeLine
{
    public string Activity { get; init; } = "";

    public int Hours { get; init; }

    public decimal Rate { get; init; }

    public string Team { get; init; } = "";
}
