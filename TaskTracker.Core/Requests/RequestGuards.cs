namespace TaskTracker.Core.Requests;

/// <summary>Перевірки передумов для даних заявки.</summary>
internal static class RequestGuards
{
    /// <summary>Перевіряє заголовок заявки: номер і контакти клієнта.</summary>
    /// <param name="requestId">Номер заявки, більший за нуль.</param>
    /// <param name="clientName">Ім’я клієнта, не порожнє.</param>
    /// <param name="clientEmail">Пошта клієнта з символом @.</param>
    public static void EnsureHeaderValid(int requestId, string clientName, string clientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientEmail);
        if (!clientEmail.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(clientEmail));
        }
    }

    /// <summary>Перевіряє один етап заявки.</summary>
    /// <param name="line">Етап робіт.</param>
    public static void EnsureLineValid(StageLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.StageCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Hours, nameof(line.Hours));
        ArgumentOutOfRangeException.ThrowIfNegative(line.HourlyRate, nameof(line.HourlyRate));
    }

    /// <summary>Перевіряє колекцію етапів: вона не порожня і кожен етап коректний.</summary>
    /// <param name="lines">Етапи заявки.</param>
    public static void EnsureLinesValid(IReadOnlyList<StageLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException("Заявка не містить жодного етапу.", nameof(lines));
        }

        foreach (StageLine line in lines)
        {
            EnsureLineValid(line);
        }
    }

    /// <summary>Перевіряє весь запит на розрахунок однією точкою входу.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    public static void EnsureRequestValid(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Client);
        EnsureHeaderValid(request.RequestId, request.Client.Name, request.Client.Email);
        EnsureLinesValid(request.Lines);
        ArgumentOutOfRangeException.ThrowIfNegative(request.TravelFee);
    }
}
