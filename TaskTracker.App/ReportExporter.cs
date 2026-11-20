using System.Globalization;
using TaskTracker.Core.Domain;

namespace TaskTracker.App;

/// <summary>Експорт звіту за завданням у файл (інфраструктура застосунку, не ядро).</summary>
internal static class ReportExporter
{
    /// <summary>Записує звіт; using звільняє потік навіть при винятку.</summary>
    /// <param name="path">Шлях до файлу звіту.</param>
    /// <param name="task">Завдання.</param>
    /// <param name="cost">Вартість завдання.</param>
    public static void Export(string path, WorkTask task, decimal cost)
    {
        ArgumentNullException.ThrowIfNull(task);
        string? folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }

        using var writer = new StreamWriter(path);
        writer.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Завдання {task.Id}"));
        writer.WriteLine(cost.ToString("0.00", CultureInfo.InvariantCulture));
    }
}
