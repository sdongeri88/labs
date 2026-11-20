using System.Globalization;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Parsing;

/// <summary>
/// Межа між «брудними» даними ззовні (рядок «вид;години;ставка» з файлу чи консолі)
/// та сутністю <see cref="WorkLog"/>.
/// </summary>
public static class WorkLogLineParser
{
    /// <summary>Найдовша назва виду роботи.</summary>
    public const int MaxActivityLength = 20;

    /// <summary>Найбільша погодинна ставка, грн.</summary>
    public const decimal MaxRate = 10_000m;

    private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    /// <summary>Пробує розібрати рядок без винятків.</summary>
    /// <param name="raw">Рядок вводу.</param>
    /// <param name="log">Запис часу або null, якщо рядок некоректний.</param>
    /// <returns>true, якщо рядок коректний.</returns>
    public static bool TryParse(string? raw, out WorkLog? log)
    {
        log = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        string[] p = raw.Split(';');
        string activity = p[0].Trim();
        if (p.Length != 3 || activity.Length is 0 or > MaxActivityLength)
        {
            return false;
        }

        if (!decimal.TryParse(p[1], NumberStyles.AllowDecimalPoint, _culture, out decimal hours)
            || hours <= 0m || hours > WorkLog.MaxHoursPerEntry)
        {
            return false;
        }

        if (!decimal.TryParse(p[2], NumberStyles.AllowDecimalPoint, _culture, out decimal rate)
            || rate > MaxRate)
        {
            return false;
        }

        log = new WorkLog(activity, hours, rate);
        return true;
    }
}
