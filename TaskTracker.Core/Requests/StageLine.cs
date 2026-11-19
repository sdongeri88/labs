namespace TaskTracker.Core.Requests;

/// <summary>Етап робіт у заявці: код, кількість годин і погодинна ставка.</summary>
public sealed class StageLine
{
    /// <summary>Код етапу.</summary>
    public string StageCode { get; set; } = string.Empty;

    /// <summary>Кількість годин на етап.</summary>
    public int Hours { get; set; }

    /// <summary>Погодинна ставка, грн.</summary>
    public decimal HourlyRate { get; set; }
}
