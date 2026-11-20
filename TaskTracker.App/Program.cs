using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using TaskTracker.App;
using TaskTracker.Core;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine($"TaskTracker v{VersionInfo.Current()}");

string auditPath = "audit.log";

using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});
ILogger log = factory.CreateLogger("TaskTracker.App");

#pragma warning disable CA1031 // Верхній обробник — єдине місце, де дозволено catch (Exception).
try
{
    Run(factory, args, auditPath);
    return 0;
}
catch (DomainRuleException ex)
{
    log.LogWarning("Правило {Rule}: {Message}", ex.Rule, ex.Message);
    Console.WriteLine($"Файл аудиту закрито після винятку: {IsFileClosed(auditPath)}");
    return 1;
}
catch (Exception ex)
{
    log.LogCritical(ex, "Непередбачений збій");
    return 2;
}
#pragma warning restore CA1031

static void Run(ILoggerFactory factory, string[] args, string auditPath)
{
    // Композиційний корінь: єдине місце, де створюються конкретні реалізації.
    IPricingPolicy pricing = ChoosePolicy(args.Length > 0 ? args[0] : "regular");
    var repository = new InMemoryTaskRepository();
    var service = new WorkTaskService(
        repository, pricing, new ConsoleNotifier(), factory.CreateLogger<WorkTaskService>());

    var task = new WorkTask(1, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));
    task.Start();
    service.Register(task);

    ReadLogs(service, task.Id);
    decimal total = service.CostOf(task);
    Console.WriteLine("Сума: " + total.ToString("0.00", CultureInfo.InvariantCulture));

    using var audit = new FileAuditLog(auditPath);
    audit.Write("register", task.Id);
    task.SubmitForReview();
    service.CloseTask(task.Id, 10000m);
    audit.Write("close", task.Id);
    ReportExporter.Export(Path.Combine("exports", "task-1.txt"), task, service.CostOf(task));
    audit.Write("export", task.Id);
    Console.WriteLine($"Стан завдання: {task.Status}");

    // Демонстрація: виняток усередині using — файл аудиту однаково закриється.
    service.Cancel(task.Id, "демонстрація помилки");
}

static bool IsFileClosed(string path)
{
    try
    {
        using FileStream probe = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        return true;
    }
    catch (IOException)
    {
        return false;
    }
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
