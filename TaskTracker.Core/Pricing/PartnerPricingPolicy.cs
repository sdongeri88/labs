using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Pricing;

/// <summary>Ціна для партнерів: знижка 10 %. Новий тип клієнта — новий клас.</summary>
public sealed class PartnerPricingPolicy : IPricingPolicy
{
    /// <summary>Частка знижки для партнерів.</summary>
    public const decimal Rate = 0.10m;

    /// <inheritdoc/>
    public decimal PriceOf(WorkLog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        return Math.Round(log.Cost * (1m - Rate), 2);
    }
}
