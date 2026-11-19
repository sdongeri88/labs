namespace TaskTracker.Core.Requests;

/// <summary>Результат розрахунку заявки.</summary>
/// <param name="Total">Вартість до сплати.</param>
/// <param name="Status">Новий стан заявки.</param>
public sealed record TotalResult(
    decimal Total,
    RequestStatus Status);
