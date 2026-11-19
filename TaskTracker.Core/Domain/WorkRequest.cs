using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTracker.Core.Domain;

/// <summary>
/// Заявка клієнта на виконання робіт, що складається з етапів
/// з кількістю годин і погодинною ставкою.
/// </summary>
public class WorkRequest
{
    private readonly List<string[]> _stages = new List<string[]>();

    // public string prim;   // примітка, поки не треба

    /// <summary>
    /// Створює нову заявку зі станом «нова» та поточною датою створення.
    /// </summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="clientName">Ім’я або назва клієнта.</param>
    public WorkRequest(string id, string clientName)
    {
        Id = id;
        ClientName = clientName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Ідентифікатор заявки.</summary>
    public string Id { get; private set; }

    /// <summary>Ім’я або назва клієнта, що подав заявку.</summary>
    public string ClientName { get; private set; }

    /// <summary>Поточний стан заявки.</summary>
    public int Status { get; private set; } // 0-нова,1-в роботі,2-викон,3-скасов

    /// <summary>Дата й час створення заявки.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає заявку за ідентифікатором у переданому списку.
    /// </summary>
    /// <param name="requests">Список заявок для пошуку.</param>
    /// <param name="requestId">Ідентифікатор шуканої заявки.</param>
    /// <returns>Знайдена заявка або null, якщо такої немає.</returns>
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

    /// <summary>
    /// Додає до заявки етап робіт.
    /// </summary>
    /// <param name="stageCode">Код етапу.</param>
    /// <param name="hours">Кількість годин на етап.</param>
    /// <param name="hourlyRate">Погодинна ставка, грн.</param>
    public void AddStage(string stageCode, int hours, decimal hourlyRate)
    {
        string[] stage = new string[3];
        stage[0] = stageCode;
        stage[1] = hours.ToString();
        stage[2] = hourlyRate.ToString();
        _stages.Add(stage);
    }

    /// <summary>
    /// Обчислює вартість робіт за заявкою з урахуванням знижок
    /// і податку на додану вартість.
    /// </summary>
    /// <param name="isRegularClient">Ознака постійного клієнта.</param>
    /// <returns>Вартість до сплати, заокруглена до двох знаків.</returns>
    public decimal CalculateCost(bool isRegularClient)
    {
        decimal total = 0;
        int stageCount = 0;
        for (int i = 0; i < _stages.Count; i++)
        {
            int stageHours = int.Parse(_stages[i][1]);
            decimal stageRate = decimal.Parse(_stages[i][2]);
            total = total + (stageHours * stageRate);
            stageCount = stageCount + 1;
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка

        // Знижки не сумуються: постійний клієнт отримує лише свою знижку,
        // навіть якщо сума перевищує поріг великої заявки.
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

        // ПДВ нараховується на суму вже після всіх знижок.
        total = total + (total * 0.2m);
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан заявки, якщо перехід дозволений правилами
    /// предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан заявки.</param>
    /// <returns>
    /// true, якщо перехід виконано; false, якщо він заборонений.
    /// </returns>
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

        return false;
    }

    /// <summary>
    /// Перевіряє, чи заповнена заявка коректно.
    /// </summary>
    /// <returns>true, якщо всі обов’язкові дані заявки коректні.</returns>
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

    /// <summary>
    /// Формує текстовий звіт за етапами заявки з підсумковою вартістю.
    /// </summary>
    /// <returns>Багаторядковий текст звіту.</returns>
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
        return report;
    }
}
