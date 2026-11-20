using System.Text;

namespace TaskTracker.Legacy;

public sealed class ReportBuilder
{
    public string Build(BillingRequest req, decimal discount, decimal shipping, decimal total)
    {
        var sb = new StringBuilder();
        sb.Append("Документ #").Append(req.TaskId).Append('\n');
        sb.Append("Клієнт: ").Append(req.Client.Name).Append('\n');
        foreach (var line in req.Lines!)
            sb.Append(Line(line, req.Currency));
        sb.Append(Money("Знижка", discount, req.Currency));
        sb.Append(Money("Доставка", shipping, req.Currency));
        sb.Append(Money("Разом", total, req.Currency));
        return sb.ToString();
    }

    private static string Line(TimeLine line, string cur) =>
        line.Activity + " x" + line.Hours + " = " +
        (line.Hours * line.Rate).ToString("0.00") + " " + cur + "\n";

    private static string Money(string label, decimal value, string cur) =>
        label + ": " + value.ToString("0.00") + " " + cur + "\n";
}
