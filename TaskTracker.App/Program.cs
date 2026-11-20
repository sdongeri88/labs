using System.Globalization;
using System.Text;
using TaskTracker.App;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Композиційний корінь: єдине місце, де створюються конкретні реалізації.
// Таблиця відповідності замість ланцюга if за типом клієнта (OCP).
var policies = new Dictionary<string, IPricingPolicy>
{
    ["regular"] = new DiscountPricingPolicy(0.05m),
    ["partner"] = new PartnerPricingPolicy(),
    ["internal"] = new DiscountPricingPolicy(0.30m),
};
string clientType = args.Length > 0 ? args[0] : "regular";
IPricingPolicy pricing = policies.TryGetValue(clientType, out IPricingPolicy? found)
    ? found
    : new StandardPricingPolicy();

var repository = new InMemoryTaskRepository();
var service = new WorkTaskService(repository, pricing, new ConsoleNotifier());

var task = new WorkTask(1, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));
task.Start();
task.AddLog(new WorkLog("DEV", 2m, 250m));
service.Register(task);
Console.WriteLine("Сума: " + service.CostOf(task).ToString("0.00", CultureInfo.InvariantCulture));

task.SubmitForReview();
task.Complete();
var archived = new ArchivedWorkTask(task, DateOnly.FromDateTime(DateTime.Today));
Console.WriteLine($"Архів: завдання #{archived.Id}, стан {archived.Status}");
Console.Write(WorkTaskCsvReport.BuildCsv(repository.GetAll()));
