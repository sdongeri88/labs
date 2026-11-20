namespace TaskTracker.Legacy;

public sealed class StaffDiscountRule : IDiscountRule
{
    public decimal? RateFor(TeamClient c) =>
        c.Kind == ClientKind.Staff ? PricingRules.StaffRate : null;
}
