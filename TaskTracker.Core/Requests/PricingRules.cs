namespace TaskTracker.Core.Requests;

/// <summary>Іменовані правила ціноутворення заявки (знижки, пороги, купони).</summary>
public static class PricingRules
{
    /// <summary>Кількість годин етапу, з якої діє гуртова знижка.</summary>
    public const int BulkHours = 10;

    /// <summary>Частка гуртової знижки на етап.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>Частка знижки постійному клієнту.</summary>
    public const decimal RegularClientRate = 0.07m;

    /// <summary>Сума, понад яку заявка вважається великою.</summary>
    public const decimal LargeRequestFrom = 5000m;

    /// <summary>Частка знижки на велику заявку.</summary>
    public const decimal LargeRequestRate = 0.10m;

    /// <summary>Сума, понад яку заявка вважається середньою.</summary>
    public const decimal MediumRequestFrom = 2000m;

    /// <summary>Частка знижки на середню заявку.</summary>
    public const decimal MediumRequestRate = 0.05m;

    /// <summary>Частка знижки для заявок, створених у неділю.</summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>Частка знижки за купоном <see cref="SaleCoupon"/>.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Фіксована знижка за купоном <see cref="FixedCoupon"/>, грн.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Сума, понад яку діє фіксований купон.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Сума, від якої виїзд фахівця безкоштовний.</summary>
    public const decimal FreeTravelFrom = 1000m;

    /// <summary>Код купона з відсотковою знижкою.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Код купона з фіксованою знижкою.</summary>
    public const string FixedCoupon = "MINUS200";
}
