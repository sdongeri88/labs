using System.Globalization;
using System.Text;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Reporting;

/// <summary>Форматування звіту CSV: повертає рядок і нічого не пише на диск.</summary>
public static class WorkTaskCsvReport
{
    /// <summary>Будує CSV зі стовпцями id;project;hours;status.</summary>
    /// <param name="tasks">Завдання для звіту.</param>
    /// <returns>Текст CSV.</returns>
    public static string BuildCsv(IEnumerable<WorkTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine("id;project;hours;status");
        foreach (WorkTask task in tasks)
        {
            text.AppendLine(
                culture,
                $"{task.Id};{task.ProjectCode};{task.TotalHours};{task.Status}");
        }

        return text.ToString();
    }
}
