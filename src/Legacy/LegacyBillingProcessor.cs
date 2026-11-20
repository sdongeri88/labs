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
    private readonly List<string> _log = new();

    public string Handle(BillingRequest req)
    {
        string? error = Validate(req);
        if (error != null) return error;

        _log.Add("ok " + req.TaskId);

        _tmpSum = Subtotal(req.Lines!);
        _tmpDiscount = DiscountOf(_tmpSum, req.Client);
        decimal ship = ShippingOf(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + req.TaskId + "\n";
        txt += "Клієнт: " + req.Client.Name + "\n";
        foreach (var line in req.Lines!)
        {
            txt += line.Activity + " x" + line.Hours + " = " +
                (line.Hours * line.Rate)
                    .ToString("0.00") + " " + req.Currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + req.Currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + req.Currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + req.Currency + "\n";

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }
        return txt;
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

    private static decimal DiscountOf(decimal subtotal, TeamClient client)
    {
        decimal discount = 0m;
        if (client.Kind == ClientKind.Vip)
            discount = subtotal * PricingRules.VipRate;
        else if (client.Kind == ClientKind.Staff)
            discount = subtotal * PricingRules.StaffRate;
        else if (client.ClosedTasks > PricingRules.LoyalOrders)
            discount = subtotal * PricingRules.LoyalRate;
        return Math.Min(discount, PricingRules.MaxDiscount);
    }

    private static decimal ShippingOf(decimal payable) =>
        payable < PricingRules.FreeShipFrom ? PricingRules.ShipCost : 0m;

    public decimal Preview(TeamClient client, IReadOnlyList<TimeLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = DiscountOf(subtotal, client);
        decimal payable = subtotal - discount;
        return payable + ShippingOf(payable);
    }

    public string DescribeClient(TeamClient c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip) s += " [VIP]";
        if (c.ClosedTasks > PricingRules.LoyalOrders) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
