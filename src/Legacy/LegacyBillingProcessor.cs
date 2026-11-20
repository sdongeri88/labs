using System;
using System.Collections.Generic;

namespace TaskTracker.Legacy;

public class TeamClient
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int ClosedTasks;
    public DateTime SinceUtc;
}

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
        string clientKind, int clientClosed,
        List<TimeLine>? lines, string state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (lines != null)
        {
            if (lines.Count > 0)
            {
                if (state == "new" || state == "paid")
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
        if (clientKind == "vip")
        {
            _tmpDiscount = _tmpSum * 0.15m;
            if (_tmpDiscount > 500m) _tmpDiscount = 500m;
        }
        else if (clientKind == "staff")
        {
            _tmpDiscount = _tmpSum * 0.30m;
            if (_tmpDiscount > 500m) _tmpDiscount = 500m;
        }
        else if (clientClosed > 10)
        {
            _tmpDiscount = _tmpSum * 0.05m;
            if (_tmpDiscount > 500m) _tmpDiscount = 500m;
        }

        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < 1000m) ship = 60m;
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

    public decimal Preview(string clientKind, int clientClosed,
        List<TimeLine> lines)
    {
        decimal s = 0m;
        foreach (var it in lines)
        {
            s += it.Hours * it.Rate;
        }
        decimal d = 0m;
        if (clientKind == "vip")
        {
            d = s * 0.15m;
            if (d > 500m) d = 500m;
        }
        else if (clientKind == "staff")
        {
            d = s * 0.30m;
            if (d > 500m) d = 500m;
        }
        else if (clientClosed > 10)
        {
            d = s * 0.05m;
            if (d > 500m) d = 500m;
        }
        decimal sh = 0m;
        if (s - d < 1000m) sh = 60m;
        return s - d + sh;
    }

    public string DescribeClient(TeamClient c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == "vip") s += " [VIP]";
        if (c.ClosedTasks > 10) s += " [ЛОЯЛЬНИЙ]";
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
