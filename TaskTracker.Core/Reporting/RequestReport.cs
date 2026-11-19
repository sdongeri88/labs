using System.Globalization;
using System.Text;
using TaskTracker.Core.Requests;

namespace TaskTracker.Core.Reporting;

/// <summary>Формування текстового звіту за заявкою (чиста функція).</summary>
public static class RequestReport
{
    /// <summary>Будує текст звіту за запитом і результатом розрахунку.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <param name="result">Результат розрахунку.</param>
    /// <returns>Багаторядковий текст звіту.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine(culture, $"Заявка № {request.RequestId}");
        text.AppendLine(culture, $"Клієнт: {request.Client.Name}");
        foreach (StageLine line in request.Lines)
        {
            text.AppendLine(culture, $"{line.StageCode} x {line.Hours}");
        }

        text.AppendLine(culture, $"Разом: {result.Total:F2}");
        text.AppendLine(culture, $"Стан: {result.Status}");
        return text.ToString();
    }
}
