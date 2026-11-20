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
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount), "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isRegular)
        {
            rate += RegularRate;
        }

        if (amount >= BigInvoiceFrom)
        {
            rate += BigInvoiceRate;
        }

        decimal discount = decimal.Round(amount * rate, 2);
        if (discount > MaxDiscount)
        {
            discount = MaxDiscount;
        }

        return discount;
    }
}
