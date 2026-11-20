using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Pricing;

/// <summary>Звичайна ціна без знижок.</summary>
public sealed class StandardPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(WorkLog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        return Math.Round(log.Cost, 2, MidpointRounding.AwayFromZero);
    }
}
