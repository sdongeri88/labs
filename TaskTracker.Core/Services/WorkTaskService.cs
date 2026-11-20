using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс завдань: сценарії реєстрації, обліку часу й закриття.</summary>
public sealed partial class WorkTaskService
{
    /// <summary>Адресат службових сповіщень про нові завдання.</summary>
    public const string LeadRecipient = "team-lead";

    private readonly ITaskRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Створює сервіс із залежностями, переданими ззовні.</summary>
    /// <param name="repository">Сховище завдань.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    /// <param name="notifier">Сповіщувач.</param>
    public WorkTaskService(
        ITaskRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>Реєструє завдання у сховищі й сповіщає керівника.</summary>
    /// <param name="task">Завдання.</param>
    public void Register(WorkTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        _repository.Add(task);
        _notifier.Notify(LeadRecipient, $"Завдання #{task.Id} ({task.ProjectCode}) зареєстровано");
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
