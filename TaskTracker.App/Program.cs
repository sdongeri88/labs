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

#pragma warning disable CA1031 // Верхній обробник — єдине місце, де дозволено catch (Exception).
try
{
    Run(args);
    return 0;
}
catch (DomainRuleException ex)
{
    Console.WriteLine($"Порушено правило {ex.Rule}: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.WriteLine($"Непередбачений збій: {ex}");
    return 2;
}
#pragma warning restore CA1031

static void Run(string[] args)
{
    // Композиційний корінь: єдине місце, де створюються конкретні реалізації.
    IPricingPolicy pricing = ChoosePolicy(args.Length > 0 ? args[0] : "regular");
    var repository = new InMemoryTaskRepository();
    var service = new WorkTaskService(repository, pricing, new ConsoleNotifier());

    var task = new WorkTask(1, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));
    task.Start();
    service.Register(task);

    ReadLogs(service, task.Id);
    decimal total = service.CostOf(task);
    Console.WriteLine("Сума: " + total.ToString("0.00", CultureInfo.InvariantCulture));

    task.SubmitForReview();
    service.CloseTask(task.Id, 10000m);
    Console.WriteLine($"Стан завдання: {task.Status}");

    // Демонстрація: завершене завдання скасувати не можна — виняток до верхнього обробника.
    service.Cancel(task.Id, "демонстрація помилки");
}

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
