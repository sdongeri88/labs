using TaskTracker.Core.Abstractions;

namespace TaskTracker.Core.Pricing;

/// <summary>
/// Знижка на рахунок за виконане завдання: постійному замовникові та за великий рахунок.
/// </summary>
public sealed class InvoiceDiscountPolicy : IInvoiceDiscountPolicy
{
    /// <summary>Знижка постійному замовникові.</summary>
    public const decimal RegularRate = 0.05m;

    /// <summary>Додаткова знижка за великий рахунок.</summary>
    public const decimal BigInvoiceRate = 0.10m;

    /// <summary>Межа великого рахунку, грн.</summary>
    public const decimal BigInvoiceFrom = 1000m;

    /// <summary>Максимальна сума знижки, грн.</summary>
    public const decimal MaxDiscount = 150m;

    /// <inheritdoc/>
    public decimal DiscountFor(decimal amount, bool isRegular)
    {
        EnsureAmountIsValid(amount);
        decimal raw = amount * RateFor(amount, isRegular);
        return Math.Min(decimal.Round(raw, 2, MidpointRounding.AwayFromZero), MaxDiscount);
    }

    private static void EnsureAmountIsValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount), "Сума не може бути від’ємною.");
        }
    }

    private static decimal RateFor(decimal amount, bool isRegular)
    {
        decimal rate = isRegular ? RegularRate : 0m;
        if (amount >= BigInvoiceFrom)
        {
            rate += BigInvoiceRate;
        }

        return rate;
    }
}
