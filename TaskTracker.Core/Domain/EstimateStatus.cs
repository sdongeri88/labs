namespace TaskTracker.Core.Domain;

/// <summary>Стан виконання завдання відносно його оцінки трудомісткості.</summary>
public enum EstimateStatus
{
    /// <summary>Витрачено менше ніж 80 % оцінки.</summary>
    WithinEstimate = 0,

    /// <summary>Витрачено від 80 % до 100 % оцінки включно — час попередити керівника.</summary>
    NearLimit = 1,

    /// <summary>Витрачено більше за оцінку.</summary>
    Overrun = 2,
}
