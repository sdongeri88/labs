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
        Email is not null && Email.Contains("@");

    public bool IsLoyal =>
        ClosedTasks > PricingRules.LoyalOrders;
}
