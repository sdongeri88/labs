using System.Text;
using TaskTracker.Application.Services;
using TaskTracker.Domain;

namespace TaskTracker.Cli;

/// <summary>Друкує звіт «завантаження виконавців» таблицею.</summary>
internal static class WorkloadPrinter
{
    private static CompositeFormat RowFormat { get; } =
        CompositeFormat.Parse("{0,-8} {1,-16} {2,8} {3,6} {4,9} {5,5}  {6}");

    /// <summary>Повертає рядки звіту для консолі.</summary>
    /// <param name="report">Звіт.</param>
    /// <returns>Заголовок, рядки виконавців і підсумок.</returns>
    public static IEnumerable<string> Lines(WorkloadReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        string date = report.BuiltOn.ToString("dd.MM.yyyy", TaskPrinter.Ukrainian);
        yield return "Завантаження виконавців на " + date;
        yield return string.Format(
            TaskPrinter.Ukrainian,
            RowFormat,
            "Логін",
            "Ім'я",
            "Активних",
            "Годин",
            "Місткість",
            "%",
            "Рівень");
        foreach (MemberWorkload row in report.Rows)
        {
            yield return string.Format(
                TaskPrinter.Ukrainian,
                RowFormat,
                row.Login,
                row.Name,
                row.ActiveTasks,
                row.ActiveHours,
                row.CapacityHours,
                row.LoadPercent,
                LevelText(row.Level));
        }

        string team = report.TeamLoadPercent.ToString("0", TaskPrinter.Ukrainian);
        yield return "Завантаження команди: " + team + " %";
    }

    private static string LevelText(WorkloadLevel level) => level switch
    {
        WorkloadLevel.Free => "вільний",
        WorkloadLevel.Normal => "нормально",
        WorkloadLevel.Overloaded => "ПЕРЕВАНТАЖЕНИЙ",
        _ => level.ToString(),
    };
}
