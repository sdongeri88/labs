namespace TaskTracker.Core.Requests;

/// <summary>Розрахунок вартості заявки на виконання робіт.</summary>
public static class RequestTotalCalculator
{
    /// <summary>Обчислює вартість заявки, змінює її стан і за потреби друкує звіт.</summary>
    /// <param name="requestId">Номер заявки.</param>
    /// <param name="clientName">Ім’я клієнта.</param>
    /// <param name="clientEmail">Пошта клієнта.</param>
    /// <param name="isRegular">Ознака постійного клієнта.</param>
    /// <param name="lines">Етапи робіт.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <param name="status">Поточний стан заявки.</param>
    /// <param name="printToConsole">Чи друкувати звіт у консоль.</param>
    /// <param name="travelFee">Плата за виїзд фахівця.</param>
    /// <param name="newStatus">Новий стан заявки.</param>
    /// <returns>Вартість до сплати.</returns>
    public static decimal Process(
        int requestId,
        string clientName,
        string clientEmail,
        bool isRegular,
        IReadOnlyList<StageLine> lines,
        DateOnly createdAt,
        string? couponCode,
        RequestStatus status,
        bool printToConsole,
        decimal travelFee,
        out RequestStatus newStatus)
    {
        RequestGuards.EnsureHeaderValid(requestId, clientName, clientEmail);
        decimal total = SubtotalOf(lines);
        total -= ClientDiscountOf(total, isRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, couponCode);
        total -= WeekendDiscountOf(total, createdAt);
        total += TravelFeeOf(total, travelFee);
        total = status == RequestStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);
        newStatus = NextStatusOf(status, total);

        if (printToConsole)
        {
            Console.WriteLine("Заявка № " + requestId);
            Console.WriteLine("Клієнт: " + clientName);
            foreach (StageLine line in lines)
            {
                Console.WriteLine(line.StageCode + " x " + line.Hours);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }

    /// <summary>Сума етапів із гуртовою знижкою на великі етапи.</summary>
    /// <param name="lines">Етапи робіт.</param>
    /// <returns>Підсумок етапів до інших знижок.</returns>
    public static decimal SubtotalOf(IReadOnlyList<StageLine> lines)
    {
        RequestGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (StageLine line in lines)
        {
            decimal sum = line.Hours * line.HourlyRate;
            if (line.Hours >= PricingRules.BulkHours)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Знижка постійного клієнта.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <param name="isRegularClient">Ознака постійного клієнта.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient ? amount * PricingRules.RegularClientRate : 0m;
    }

    /// <summary>Знижка за обсягом заявки.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.LargeRequestFrom)
        {
            return amount * PricingRules.LargeRequestRate;
        }

        if (amount > PricingRules.MediumRequestFrom)
        {
            return amount * PricingRules.MediumRequestRate;
        }

        return 0m;
    }

    /// <summary>Знижка за кодом купона.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Знижка для заявки, створеної в неділю.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
            ? amount * PricingRules.WeekendRate
            : 0m;
    }

    /// <summary>Плата за виїзд фахівця: стягується лише з невеликих заявок.</summary>
    /// <param name="amount">Сума заявки після знижок.</param>
    /// <param name="travelFee">Тариф виїзду.</param>
    /// <returns>Плата за виїзд, грн.</returns>
    public static decimal TravelFeeOf(decimal amount, decimal travelFee)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(travelFee);
        return amount < PricingRules.FreeTravelFrom ? travelFee : 0m;
    }

    /// <summary>Наступний стан заявки після розрахунку.</summary>
    /// <param name="current">Поточний стан.</param>
    /// <param name="total">Підсумкова вартість.</param>
    /// <returns>Новий стан заявки.</returns>
    public static RequestStatus NextStatusOf(RequestStatus current, decimal total)
        => current switch
        {
            RequestStatus.New when total > 0m => RequestStatus.Approved,
            RequestStatus.Approved => RequestStatus.Completed,
            _ => current,
        };
}
