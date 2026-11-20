using System.Globalization;

namespace TaskTracker.Legacy;

public sealed class TeamClient
{
    public int Id { get; init; }

    public string Name { get; init; } = "";

    public string Email { get; init; } = "";

    public ClientKind Kind { get; init; }

    public int ClosedTasks { get; init; }

    public DateTime SinceUtc { get; init; }

    public bool HasValidEmail =>
        Email is not null && Email.Contains('@');

    public bool IsLoyal =>
        ClosedTasks > PricingRules.LoyalOrders;

    // регістр змінюється за поточною культурою, як і до ЛР8 (поведінку збережено, залежність явна)
    public string Describe()
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        string s = Name.Trim().ToUpper(culture);
        if (Kind == ClientKind.Vip)
        {
            s += " [VIP]";
        }

        if (IsLoyal)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + Email.ToLower(culture) + ">";
        return s + " стаж " + (DateTime.Now.Year - SinceUtc.Year);
    }
}
