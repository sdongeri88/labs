using System.Globalization;
using System.Text;

namespace TaskTracker.Legacy;

public static class ReportBuilder
{
    // ЛР8: суми форматуються з InvariantCulture — погашено борг TD-01 (кома в uk-UA)
    private static readonly CultureInfo _money = CultureInfo.InvariantCulture;

    public static string Build(
        BillingRequest req, decimal discount, decimal shipping, decimal total)
    {
        var sb = new StringBuilder();
        sb.Append("Документ #").Append(req.TaskId).Append('\n');
        sb.Append("Клієнт: ").Append(req.Client.Name).Append('\n');
        foreach (var line in req.Lines!)
        {
            sb.Append(Line(line, req.Currency));
        }

        sb.Append(Amount("Знижка", discount, req.Currency));
        sb.Append(Amount("Доставка", shipping, req.Currency));
        sb.Append(Amount("Разом", total, req.Currency));
        return sb.ToString();
    }

    private static string Line(TimeLine line, string cur) =>
        line.Activity + " x" + line.Hours + " = " +
        (line.Hours * line.Rate).ToString("0.00", _money) + " " + cur + "\n";

    private static string Amount(string label, decimal value, string cur) =>
        label + ": " + value.ToString("0.00", _money) + " " + cur + "\n";
}
