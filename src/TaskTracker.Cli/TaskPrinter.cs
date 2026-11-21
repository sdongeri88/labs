using System.Globalization;
using TaskTracker.Domain;

namespace TaskTracker.Cli;

/// <summary>Форматує завдання для консолі.</summary>
internal static class TaskPrinter
{
    /// <summary>Культура виводу чисел: кома як десятковий роздільник.</summary>
    public static CultureInfo Ukrainian { get; } = CultureInfo.GetCultureInfo("uk-UA");

    /// <summary>Один рядок про завдання.</summary>
    /// <param name="task">Завдання.</param>
    /// <returns>Рядок «#номер [колонка] назва (пріоритет, години) @виконавець».</returns>
    public static string Line(BoardTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        string hours = task.EstimateHours.ToString("0.#", Ukrainian);
        string who = task.AssigneeLogin is null ? "без виконавця" : "@" + task.AssigneeLogin;
        return string.Format(
            Ukrainian,
            "#{0,-3} [{1,-10}] {2} ({3}, {4} год) {5}",
            task.Id,
            task.Column,
            task.Title,
            task.Priority,
            hours,
            who);
    }
}
