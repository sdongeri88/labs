using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Pricing;

/// <summary>Ціна зі знижкою у відсотках (від 0 до 50 %).</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    /// <summary>Найбільша допустима частка знижки.</summary>
    public const decimal MaxRate = 0.5m;

    private readonly decimal _rate;

    /// <summary>Створює політику з указаною часткою знижки.</summary>
    /// <param name="rate">Частка знижки від 0 до 0,5.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rate, MaxRate);
        _rate = rate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(WorkLog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        return Math.Round(log.Cost * (1m - _rate), 2);
    }
}
