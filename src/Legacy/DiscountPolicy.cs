namespace TaskTracker.Legacy;

public sealed class DiscountPolicy
{
    // порядок важливий: вид клієнта перевіряється раніше за лояльність
    private readonly IReadOnlyList<IDiscountRule> _rules = new IDiscountRule[]
    {
        new VipDiscountRule(),
        new StaffDiscountRule(),
        new LoyalDiscountRule(),
    };

    public decimal For(decimal subtotal, TeamClient c)
    {
        decimal rate = RateFor(c);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private decimal RateFor(TeamClient c)
    {
        foreach (var rule in _rules)
        {
            decimal? rate = rule.RateFor(c);
            if (rate.HasValue)
            {
                return rate.Value;
            }
        }

        return 0m;
    }
}
