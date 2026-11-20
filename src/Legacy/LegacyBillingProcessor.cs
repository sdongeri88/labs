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

    public string Handle(int taskId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientClosed,
        List<TimeLine>? lines, BillState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (lines != null)
        {
            if (lines.Count > 0)
            {
                if (state == BillState.New || state == BillState.Paid)
                {
                    if (clientMail != null &&
                        clientMail.Contains("@"))
                    {
                        _log.Add("ok " + taskId);
                    }
                    else
                    {
                        return "ERR: mail";
                    }
                }
                else
                {
                    return "ERR: state";
                }
            }
            else
            {
                return "ERR: empty";
            }
        }
        else
        {
            return "ERR: null";
        }

        _tmpSum = 0m;
        for (int i = 0; i < lines.Count; i++)
        {
            _tmpSum += lines[i].Hours * lines[i].Rate;
        }

        _tmpDiscount = 0m;
        if (clientKind == ClientKind.Vip)
        {
            _tmpDiscount = _tmpSum * PricingRules.VipRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            _tmpDiscount = _tmpSum * PricingRules.StaffRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (clientClosed > PricingRules.LoyalOrders)
        {
            _tmpDiscount = _tmpSum * PricingRules.LoyalRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }

        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < PricingRules.FreeShipFrom) ship = PricingRules.ShipCost;
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + taskId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < lines.Count; i++)
        {
            txt += lines[i].Activity + " x" + lines[i].Hours + " = " +
                (lines[i].Hours * lines[i].Rate)
                    .ToString("0.00") + " " + currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";

        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }
        return txt;
    }

    public decimal Preview(ClientKind clientKind, int clientClosed,
        List<TimeLine> lines)
    {
        decimal s = 0m;
        foreach (var it in lines)
        {
            s += it.Hours * it.Rate;
        }
        decimal d = 0m;
        if (clientKind == ClientKind.Vip)
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (clientClosed > PricingRules.LoyalOrders)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        decimal sh = 0m;
        if (s - d < PricingRules.FreeShipFrom) sh = PricingRules.ShipCost;
        return s - d + sh;
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
