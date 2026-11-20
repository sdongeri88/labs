using System.Globalization;
using System.Text;
using TaskTracker.App;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Композиційний корінь: єдине місце, де створюються конкретні реалізації.
IPricingPolicy pricing = ChoosePolicy(args.Length > 0 ? args[0] : "regular");
var repository = new InMemoryTaskRepository();
var service = new WorkTaskService(repository, pricing, new ConsoleNotifier());

var task = new WorkTask(1, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));
task.Start();
service.Register(task);

ReadLogs(service, task.Id);
Console.WriteLine("Сума: " + service.CostOf(task).ToString("0.00", CultureInfo.InvariantCulture));

task.SubmitForReview();
service.CloseTask(task.Id, 10000m);
Console.WriteLine($"Стан завдання: {task.Status}");

static IPricingPolicy ChoosePolicy(string clientType)
{
    // Таблиця відповідності замість ланцюга if за типом клієнта (OCP).
    var policies = new Dictionary<string, IPricingPolicy>
    {
        ["regular"] = new DiscountPricingPolicy(0.05m),
        ["partner"] = new PartnerPricingPolicy(),
        ["internal"] = new DiscountPricingPolicy(0.30m),
    };
    return policies.TryGetValue(clientType, out IPricingPolicy? found)
        ? found
        : new StandardPricingPolicy();
}

static void ReadLogs(WorkTaskService service, int taskId)
{
    while (true)
    {
        Console.Write("Запис часу (вид;години;ставка) або end: ");
        string? line = Console.ReadLine();
        if (Console.IsInputRedirected)
        {
            Console.WriteLine(line);
        }

        if (line is null || line.Trim() == "end")
        {
            return;
        }

        Result<WorkLog> parsed = WorkLogParser.ParseLine(line);
        if (!parsed.IsSuccess)
        {
            Console.WriteLine($"Помилка вводу: {parsed.Error}");
            continue;
        }

        service.AddLog(taskId, parsed.Value!);
    }
}
