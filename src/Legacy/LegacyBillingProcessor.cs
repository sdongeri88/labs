using System;
using System.Collections.Generic;

namespace TaskTracker.Legacy;

public class TimeLine
{
    public string Activity = "";
    public int Hours;
    public decimal Rate;
    public string Team = "";
}

public class LegacyBillingProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private readonly List<string> _log = new();

    public string Handle(BillingRequest req)
    {
        string? error = Validate(req);
        if (error != null) return error;

        _log.Add("ok " + req.TaskId);

        _tmpSum = Subtotal(req.Lines!);
        _tmpDiscount = _discounts.For(_tmpSum, req.Client);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }
        return _report.Build(req, _tmpDiscount, ship, total);
    }

    private static string? Validate(BillingRequest req)
    {
        if (req.Lines is null) return "ERR: null";
        if (req.Lines.Count == 0) return "ERR: empty";
        if (req.State is not (BillState.New or BillState.Paid)) return "ERR: state";
        if (!req.Client.HasValidEmail) return "ERR: mail";
        return null;
    }

    private static decimal Subtotal(IReadOnlyList<TimeLine> lines)
    {
        decimal sum = 0m;
        foreach (var it in lines) sum += it.Hours * it.Rate;
        return sum;
    }

    public decimal Preview(TeamClient client, IReadOnlyList<TimeLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = _discounts.For(subtotal, client);
        decimal payable = subtotal - discount;
        return payable + _shipping.For(payable);
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
