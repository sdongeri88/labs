namespace TaskTracker.Legacy;

public sealed class LoyalDiscountRule : IDiscountRule
{
    public decimal? RateFor(TeamClient c) =>
        c.IsLoyal ? PricingRules.LoyalRate : null;
}
