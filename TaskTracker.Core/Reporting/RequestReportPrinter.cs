namespace TaskTracker.Core.Reporting;

/// <summary>Виведення готового звіту в потік (команда без результату).</summary>
public sealed class RequestReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Створює принтер, що пише у вказаний потік.</summary>
    /// <param name="output">Потік виведення, наприклад Console.Out.</param>
    public RequestReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Записує звіт у потік виведення.</summary>
    /// <param name="report">Текст звіту, не порожній.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
