using System.Globalization;
using System.Text;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Композиційний корінь: єдине місце, де створюються конкретні реалізації.
var repository = new InMemoryTaskRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
var service = new WorkTaskService(repository, pricing);

var task = new WorkTask(1, "CRM", 10, DateOnly.FromDateTime(DateTime.Today));
task.Start();
task.AddLog(new WorkLog("DEV", 2m, 250m));
service.Register(task);

Console.WriteLine("Сума: " + service.CostOf(task).ToString("0.00", CultureInfo.InvariantCulture));
