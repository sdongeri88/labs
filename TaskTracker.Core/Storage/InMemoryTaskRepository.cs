using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Storage;

/// <summary>Сховище завдань у пам’яті (словник за ідентифікатором).</summary>
public sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly Dictionary<int, WorkTask> _tasks = new();

    /// <inheritdoc/>
    public void Add(WorkTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (!_tasks.TryAdd(task.Id, task))
        {
            throw new InvalidOperationException($"Завдання #{task.Id} уже є у сховищі.");
        }
    }

    /// <inheritdoc/>
    public WorkTask? GetById(int id) => _tasks.GetValueOrDefault(id);

    /// <inheritdoc/>
    public IReadOnlyList<WorkTask> GetAll() => _tasks.Values.ToList();
}
