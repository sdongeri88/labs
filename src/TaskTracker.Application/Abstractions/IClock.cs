namespace TaskTracker.Application.Abstractions;

/// <summary>Джерело поточного часу; у тестах підмінюється фіксованим годинником.</summary>
public interface IClock
{
    /// <summary>Поточний момент.</summary>
    DateTimeOffset Now { get; }

    /// <summary>Поточна дата.</summary>
    DateOnly Today { get; }
}
