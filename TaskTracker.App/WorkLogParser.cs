using System.Globalization;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

namespace TaskTracker.App;

/// <summary>
/// Барикада: перетворює сирі рядки з консолі на перевірений <see cref="WorkLog"/>.
/// Числа очікуються з крапкою як роздільником дробової частини (InvariantCulture).
/// </summary>
internal static class WorkLogParser
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    /// <summary>Розбирає рядок формату «вид;години;ставка».</summary>
    /// <param name="line">Рядок вводу.</param>
    /// <returns>Запис часу або пояснення помилки.</returns>
    public static Result<WorkLog> ParseLine(string? line)
    {
        string[] parts = (line ?? string.Empty).Split(';');
        if (parts.Length != 3)
        {
            return Result.Fail<WorkLog>("Формат: вид;години;ставка");
        }

        return Parse(parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
    }

    /// <summary>Розбирає вид роботи, години й ставку.</summary>
    /// <param name="activity">Вид роботи.</param>
    /// <param name="hours">Години як текст.</param>
    /// <param name="rate">Ставка як текст.</param>
    /// <returns>Запис часу або пояснення помилки.</returns>
    public static Result<WorkLog> Parse(string? activity, string? hours, string? rate)
    {
        if (string.IsNullOrWhiteSpace(activity))
        {
            return Result.Fail<WorkLog>("Вид роботи порожній");
        }

        if (!decimal.TryParse(hours, NumberStyles.AllowDecimalPoint, Culture, out decimal h)
            || h <= 0m || h > WorkLog.MaxHoursPerEntry)
        {
            return Result.Fail<WorkLog>(
                "Години — число більше 0 і не більше 24 (роздільник — крапка)");
        }

        if (!decimal.TryParse(rate, NumberStyles.AllowDecimalPoint, Culture, out decimal r) || r < 0m)
        {
            return Result.Fail<WorkLog>("Ставка — невід’ємне число");
        }

        return Result.Ok(new WorkLog(activity, h, r));
    }
}
