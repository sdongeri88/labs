using System.Globalization;

namespace TaskTracker.Core.Domain;

/// <summary>
/// Заявка клієнта на виконання робіт, що складається з етапів
/// з кількістю годин і погодинною ставкою.
/// </summary>
public class WorkRequest
{
    /// <summary>Стан «нова»: заявку створено, роботи не розпочато.</summary>
    public const int StatusNew = 0;

    /// <summary>Стан «в роботі».</summary>
    public const int StatusInProgress = 1;

    /// <summary>Стан «виконана».</summary>
    public const int StatusDone = 2;

    /// <summary>Стан «скасована».</summary>
    public const int StatusCancelled = 3;

    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному клієнту.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному клієнту.</summary>
    private const decimal RegularDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на велику заявку.</summary>
    private const decimal LargeRequestThreshold = 5000m;

    /// <summary>Частка знижки на велику заявку.</summary>
    private const decimal LargeRequestDiscountRate = 0.15m;

    /// <summary>Кількість етапів, після якої діє гуртова знижка.</summary>
    private const int BulkStageCount = 10;

    /// <summary>Сума гуртової знижки, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Межа кількості етапів у заявці (не включно).</summary>
    private const int MaxStageCount = 100;

    /// <summary>Найкоротше допустиме ім’я клієнта.</summary>
    private const int MinClientNameLength = 3;

    private readonly List<(string Code, int Hours, decimal Rate)> _stages =
        new List<(string Code, int Hours, decimal Rate)>();

    /// <summary>
    /// Створює нову заявку зі станом «нова» та поточною датою створення.
    /// </summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="clientName">Ім’я або назва клієнта.</param>
    public WorkRequest(string id, string clientName)
    {
        Id = id;
        ClientName = clientName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Ідентифікатор заявки.</summary>
    public string Id { get; private set; }

    /// <summary>Ім’я або назва клієнта, що подав заявку.</summary>
    public string ClientName { get; private set; }

    /// <summary>Поточний стан заявки (одна з констант Status*).</summary>
    public int Status { get; private set; }

    /// <summary>Дата й час створення заявки.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає заявку за ідентифікатором у переданому списку.
    /// </summary>
    /// <param name="requests">Список заявок для пошуку.</param>
    /// <param name="requestId">Ідентифікатор шуканої заявки.</param>
    /// <returns>Знайдена заявка або null, якщо такої немає.</returns>
    public static WorkRequest? FindById(List<WorkRequest> requests, string requestId)
    {
        ArgumentNullException.ThrowIfNull(requests);
        for (int i = 0; i < requests.Count; i++)
        {
            if (requests[i].Id == requestId)
            {
                return requests[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до заявки етап робіт.
    /// </summary>
    /// <param name="stageCode">Код етапу.</param>
    /// <param name="hours">Кількість годин на етап.</param>
    /// <param name="hourlyRate">Погодинна ставка, грн.</param>
    public void AddStage(string stageCode, int hours, decimal hourlyRate)
    {
        // ЛР5: етап зберігається типізовано — повторний Parse рядків не потрібен.
        _stages.Add((stageCode, hours, hourlyRate));
    }

    /// <summary>
    /// Обчислює вартість робіт за заявкою з урахуванням знижок
    /// і податку на додану вартість.
    /// </summary>
    /// <param name="isRegularClient">Ознака постійного клієнта.</param>
    /// <returns>Вартість до сплати, заокруглена до двох знаків.</returns>
    public decimal CalculateCost(bool isRegularClient)
    {
        decimal total = 0;
        int stageCount = 0;
        for (int i = 0; i < _stages.Count; i++)
        {
            total += _stages[i].Hours * _stages[i].Rate;
            stageCount++;
        }

        // Знижки не сумуються: постійний клієнт отримує лише свою знижку,
        // навіть якщо сума перевищує поріг великої заявки.
        if (isRegularClient && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeRequestThreshold)
        {
            total *= 1m - LargeRequestDiscountRate;
        }

        if (stageCount > BulkStageCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок.
        total += total * VatRate;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан заявки, якщо перехід дозволений правилами
    /// предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан заявки.</param>
    /// <returns>
    /// true, якщо перехід виконано; false, якщо він заборонений.
    /// </returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusInProgress)
        {
            Status = StatusInProgress;
            return true;
        }

        if (Status == StatusInProgress && newStatus == StatusDone)
        {
            Status = StatusDone;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи заповнена заявка коректно.
    /// </summary>
    /// <returns>true, якщо всі обов’язкові дані заявки коректні.</returns>
    public bool IsValid()
    {
        if (Id is not null && Id.Length > 0
            && ClientName is not null
            && ClientName.Length >= MinClientNameLength
            && _stages.Count > 0
            && _stages.Count < MaxStageCount
            && Status >= StatusNew
            && Status <= StatusCancelled)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Формує текстовий звіт за етапами заявки з підсумковою вартістю.
    /// </summary>
    /// <returns>Багаторядковий текст звіту.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _stages.Count; i++)
        {
            (string code, int hours, decimal rate) = _stages[i];
            report = report + "Етап: " + code
                + "; годин: " + hours.ToString(CultureInfo.CurrentCulture)
                + "; ставка: " + rate.ToString(CultureInfo.CurrentCulture)
                + "; вартість: " + (hours * rate) + "\n";
        }

        report = report + "Разом: " + CalculateCost(false) + "\n";
        return report;
    }
}
