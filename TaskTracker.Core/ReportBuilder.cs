using System.Text;

namespace TaskTracker.Core;

/// <summary>Формує текст простого звіту з готових рядків.</summary>
public sealed class ReportBuilder
{
    /// <summary>Заголовок звіту.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Будує звіт: заголовок великими літерами, далі рядки.</summary>
    /// <param name="rows">Рядки звіту.</param>
    /// <returns>Текст звіту.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (var row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }

    /// <summary>Підпис із кількістю позицій (навмисне порушення для перевірки конвеєра).</summary>
    /// <param name="count">Кількість позицій.</param>
    /// <returns>Підпис.</returns>
    public string Describe(int count)
    {
        string note;
        return "Позицій: " + count;
    }
}
