using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс завдань: сценарій реєстрації та розрахунок вартості.</summary>
public sealed class WorkTaskService
{
    private readonly ITaskRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Створює сервіс із залежностями, переданими ззовні.</summary>
    /// <param name="repository">Сховище завдань.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    public WorkTaskService(ITaskRepository repository, IPricingPolicy pricing)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    /// <summary>Реєструє завдання у сховищі.</summary>
    /// <param name="task">Завдання.</param>
    public void Register(WorkTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        _repository.Add(task);
    }

    /// <summary>Обчислює вартість завдання за поточною політикою.</summary>
    /// <param name="task">Завдання.</param>
    /// <returns>Вартість, грн.</returns>
    public decimal CostOf(WorkTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        decimal total = 0m;
        foreach (WorkLog log in task.Logs)
        {
            total += _pricing.PriceOf(log);
        }

        return total;
    }
}
