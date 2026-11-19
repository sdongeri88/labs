using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTracker.Core.Domain;

// клас
public class WorkRequest
{
    private readonly List<string[]> _stages = new List<string[]>();

    // public string prim;   // примітка, поки не треба

    // конструктор
    public WorkRequest(string id, string clientName)
    {
        Id = id;              // ставимо id
        ClientName = clientName;         // ставимо cl
        CreatedAt = DateTime.Now;   // ставимо дату
    }

    public string Id { get; private set; }

    public string ClientName { get; private set; }

    public int Status { get; private set; } // 0-нова,1-в роботі,2-викон,3-скасов

    public DateTime CreatedAt { get; private set; }

    // пошук
    public static WorkRequest? FindById(List<WorkRequest> requests, string requestId)
    {
        for (int i = 0; i < requests.Count; i++)
        {
            if (requests[i].Id == requestId)
            {
                return requests[i];
            }
        }

        return null;
    }

    // метод додавання
    public void AddStage(string stageCode, int hours, decimal hourlyRate)
    {
        string[] stage = new string[3];
        stage[0] = stageCode;
        stage[1] = hours.ToString();
        stage[2] = hourlyRate.ToString();
        _stages.Add(stage);   // додаємо t у ln
    }

    // ProcessData
    public decimal CalculateCost(bool isRegularClient)
    {
        decimal total = 0;
        int stageCount = 0;
        for (int i = 0; i < _stages.Count; i++)
        {
            int stageHours = int.Parse(_stages[i][1]);
            decimal stageRate = decimal.Parse(_stages[i][2]);
            total = total + (stageHours * stageRate);   // додаємо до суми
            stageCount = stageCount + 1;     // збільшуємо kolvo на одиницю
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка
        if (isRegularClient == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (stageCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = total + (total * 0.2m);
        return Math.Round(total, 2);   // повертаємо sum1
    }

    // міняємо статус
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false;   // не можна
    }

    // перевірка
    public bool IsValid()
    {
        if (Id is not null && Id.Length > 0
            && ClientName is not null
            && ClientName.Length > 2
            && _stages.Count > 0
            && _stages.Count < 100
            && Status >= 0
            && Status <= 3)
        {
            return true;
        }

        return false;
    }

    // звіт
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _stages.Count; i++)
        {
            int stageHours = int.Parse(_stages[i][1]);
            decimal stageRate = decimal.Parse(_stages[i][2]);
            report = report + "Етап: " + _stages[i][0]
                + "; годин: " + _stages[i][1]
                + "; ставка: " + _stages[i][2]
                + "; вартість: " + (stageHours * stageRate) + "\n";
        }

        report = report + "Разом: " + CalculateCost(false) + "\n";
        return report;   // повертаємо s
    }
}
