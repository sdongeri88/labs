namespace TaskTracker.Core.Requests;

/// <summary>Вхідні дані для розрахунку вартості заявки.</summary>
/// <param name="RequestId">Номер заявки.</param>
/// <param name="Client">Клієнт.</param>
/// <param name="Lines">Етапи робіт.</param>
/// <param name="CreatedAt">Дата створення заявки.</param>
/// <param name="CouponCode">Код купона або null.</param>
/// <param name="Status">Поточний стан заявки.</param>
/// <param name="TravelFee">Тариф виїзду фахівця.</param>
public sealed record TotalRequest(
    int RequestId,
    ClientInfo Client,
    IReadOnlyList<StageLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    RequestStatus Status,
    decimal TravelFee);
