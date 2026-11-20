using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

/// <summary>Фейк: спрощене, але робоче сховище завдань у пам’яті.</summary>
internal sealed class FakeTaskRepository : ITaskRepository
{
    private readonly Dictionary<int, WorkTask> _items = new();

    public int AddCalls { get; private set; }

    public void Add(WorkTask task)
    {
        _items[task.Id] = task;
        AddCalls++;
    }

    public WorkTask? GetById(int id) =>
        _items.TryGetValue(id, out WorkTask? found) ? found : null;

    public IReadOnlyList<WorkTask> GetAll() => _items.Values.ToList();
}

/// <summary>Стаб: жодного стану, лише наперед задана ціна запису часу.</summary>
internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal PriceOf(WorkLog log) => _price;
}

/// <summary>Шпигун: запам’ятовує надіслані сповіщення, щоб тест перевірив взаємодію.</summary>
internal sealed class RecordingNotifier : INotifier
{
    public List<string> Messages { get; } = new();

    public void Notify(string recipient, string message) =>
        Messages.Add(recipient + ": " + message);
}
