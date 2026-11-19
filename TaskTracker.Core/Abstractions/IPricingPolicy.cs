using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Abstractions;

/// <summary>Політика ціноутворення: як порахувати вартість одного запису часу.</summary>
public interface IPricingPolicy
{
    /// <summary>Повертає вартість запису часу за цією політикою.</summary>
    /// <param name="log">Запис часу.</param>
    /// <returns>Вартість, грн.</returns>
    decimal PriceOf(WorkLog log);
}
