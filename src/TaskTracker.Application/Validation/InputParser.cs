using System.Globalization;
using TaskTracker.Domain;

namespace TaskTracker.Application.Validation;

/// <summary>
/// Барикада: перетворює рядки з консолі на значення домену рівно один раз,
/// на вході до прикладного шару. Некоректний ввід — невдалий результат, а не виняток.
/// </summary>
public static class InputParser
{
    /// <summary>Найдовша допустима назва завдання, символів.</summary>
    public const int MaxTitleLength = 80;

    /// <summary>Найбільша допустима оцінка одного завдання, годин.</summary>
    public const decimal MaxEstimateHours = 80m;

    /// <summary>Перевіряє назву завдання.</summary>
    /// <param name="text">Введений рядок.</param>
    /// <returns>Обрізана назва або пояснення.</returns>
    public static Result<string> Title(string? text)
    {
        string title = text?.Trim() ?? string.Empty;
        if (title.Length == 0)
        {
            return Result.Fail<string>("Назва не може бути порожньою.");
        }

        return title.Length <= MaxTitleLength
            ? Result.Ok(title)
            : Result.Fail<string>($"Назва довша за {MaxTitleLength} символів.");
    }

    /// <summary>Розбирає номер завдання.</summary>
    /// <param name="text">Введений рядок.</param>
    /// <returns>Номер, більший за нуль, або пояснення.</returns>
    public static Result<int> TaskId(string? text)
    {
        bool parsed = int.TryParse(
            text?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int id);
        return parsed && id > 0
            ? Result.Ok(id)
            : Result.Fail<int>($"«{text}» не є номером завдання.");
    }

    /// <summary>Розбирає оцінку трудомісткості.</summary>
    /// <param name="text">Введений рядок; кома або крапка як роздільник.</param>
    /// <returns>Оцінка від 0 до <see cref="MaxEstimateHours"/> або пояснення.</returns>
    public static Result<decimal> Hours(string? text)
    {
        string normalized = (text ?? string.Empty).Trim().Replace(',', '.');
        bool parsed = decimal.TryParse(
            normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal hours);
        if (!parsed)
        {
            return Result.Fail<decimal>($"«{text}» не є числом годин.");
        }

        return hours >= 0m && hours <= MaxEstimateHours
            ? Result.Ok(hours)
            : Result.Fail<decimal>($"Оцінка має бути від 0 до {MaxEstimateHours} год.");
    }

    /// <summary>Розбирає пріоритет: число 1–4 або назву.</summary>
    /// <param name="text">Введений рядок; порожній означає Normal.</param>
    /// <returns>Пріоритет або пояснення.</returns>
    public static Result<TaskPriority> Priority(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result.Ok(TaskPriority.Normal);
        }

        return TryEnum(text, out TaskPriority priority)
            ? Result.Ok(priority)
            : Result.Fail<TaskPriority>($"Невідомий пріоритет «{text}».");
    }

    /// <summary>Розбирає назву колонки дошки.</summary>
    /// <param name="text">Назва колонки без урахування регістру.</param>
    /// <returns>Колонка або пояснення.</returns>
    public static Result<BoardColumn> Column(string? text)
    {
        bool isName = !string.IsNullOrWhiteSpace(text) && !char.IsDigit(text.Trim()[0]);
        return isName && TryEnum(text!, out BoardColumn column)
            ? Result.Ok(column)
            : Result.Fail<BoardColumn>($"Невідома колонка «{text}».");
    }

    private static bool TryEnum<TEnum>(string text, out TEnum value)
        where TEnum : struct, Enum =>
        Enum.TryParse(text.Trim(), ignoreCase: true, out value) && Enum.IsDefined(value);
}
