namespace TaskTracker.Legacy;

public interface IDiscountRule
{
    // ставка знижки для клієнта або null, якщо правило його не стосується
    decimal? RateFor(TeamClient c);
}
