namespace TaskTracker.Core.Abstractions;

/// <summary>Політика знижки на рахунок за виконане завдання.</summary>
public interface IInvoiceDiscountPolicy
{
    /// <summary>Обчислює знижку для суми рахунку.</summary>
    /// <param name="amount">Сума рахунку, грн (не від’ємна).</param>
    /// <param name="isRegular">Чи є замовник постійним.</param>
    /// <returns>Знижка, грн, округлена до копійок.</returns>
    decimal DiscountFor(decimal amount, bool isRegular);
}
