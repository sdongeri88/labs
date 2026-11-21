using TaskTracker.Application.Abstractions;

namespace TaskTracker.Infrastructure;

/// <summary>Системний годинник: єдине місце рішення, яке читає поточний час.</summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc/>
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <inheritdoc/>
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
