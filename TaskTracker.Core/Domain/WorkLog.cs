namespace TaskTracker.Core.Domain;

/// <summary>Запис витраченого часу: вид роботи, години й погодинна ставка.</summary>
public sealed class WorkLog
{
    public string Activity { get; init; } = string.Empty;

    public decimal Hours { get; init; }

    public decimal HourlyRate { get; init; }

    public decimal Cost => Hours * HourlyRate;
}
