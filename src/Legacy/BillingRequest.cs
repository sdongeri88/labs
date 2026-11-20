namespace TaskTracker.Legacy;

public sealed record BillingRequest(
    int TaskId,
    TeamClient Client,
    IReadOnlyList<TimeLine>? Lines,
    BillState State,
    string Currency,
    bool SendMail);
