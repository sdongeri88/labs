namespace TaskTracker.Core.Domain;

/// <summary>Запис витраченого часу: вид роботи, години й погодинна ставка.</summary>
public sealed class WorkLog
{
    /// <summary>Вид роботи.</summary>
    public string Activity { get; init; } = string.Empty;

    /// <summary>Кількість витрачених годин.</summary>
    public decimal Hours { get; init; }

    /// <summary>Погодинна ставка, грн.</summary>
    public decimal HourlyRate { get; init; }

    /// <summary>Вартість запису: години × ставка.</summary>
    public decimal Cost => Hours * HourlyRate;
}
