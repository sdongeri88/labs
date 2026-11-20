namespace TaskTracker.Legacy;

public sealed class VipDiscountRule : IDiscountRule
{
    public decimal? RateFor(TeamClient c) =>
        c.Kind == ClientKind.Vip ? PricingRules.VipRate : null;
}
