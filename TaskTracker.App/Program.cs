using System.Globalization;
using System.Text;
using TaskTracker.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

WorkTask task = new()
{
    Id = 1,
    ProjectCode = "CRM",
    AssigneeId = 10,
    CreatedAt = DateTimeOffset.Now,
};

task.AddLog(new WorkLog
{
    Activity = "DEV",
    Hours = 4.5m,
    HourlyRate = 400m,
});
task.AddLog(new WorkLog
{
    Activity = "REVIEW",
    Hours = 1.5m,
    HourlyRate = 300m,
});

string hours = task.TotalHours()
    .ToString("F2", CultureInfo.InvariantCulture);
string cost = task.TotalCost()
    .ToString("F2", CultureInfo.InvariantCulture);
Console.WriteLine($"Завдання #{task.Id} (проєкт {task.ProjectCode})");
Console.WriteLine($"Стан: {task.Status}");
Console.WriteLine($"Записів часу: {task.Logs.Count}");
Console.WriteLine($"Годин: {hours}");
Console.WriteLine($"Вартість: {cost}");
