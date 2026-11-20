using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

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

    /// <summary>Додає запис часу до завдання, що перебуває в роботі.</summary>
    /// <param name="taskId">Ідентифікатор завдання.</param>
    /// <param name="log">Перевірений запис часу.</param>
    public void AddLog(int taskId, WorkLog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        WorkTask task = _repository.GetById(taskId)
            ?? throw new DomainRuleException("task.exists", $"Завдання {taskId} не знайдено");
        if (task.Status != WorkTaskStatus.InProgress)
        {
            throw new DomainRuleException(
                "task.editable", $"Стан {task.Status} не дозволяє облік часу завдання {taskId}");
        }

        task.AddLog(log);
    }

    /// <summary>Закриває перевірене завдання в межах погодженого бюджету.</summary>
    /// <param name="taskId">Ідентифікатор завдання.</param>
    /// <param name="approvedBudget">Погоджений бюджет, грн.</param>
    public void CloseTask(int taskId, decimal approvedBudget)
    {
        WorkTask task = _repository.GetById(taskId)
            ?? throw new DomainRuleException("task.exists", $"Завдання {taskId} не знайдено");
        if (task.Status != WorkTaskStatus.Review)
        {
            throw new DomainRuleException(
                "task.closable", $"Стан {task.Status} не дозволяє закрити завдання {taskId}");
        }

        decimal cost = CostOf(task);
        if (approvedBudget < cost)
        {
            throw new DomainRuleException(
                "task.budget", $"Погоджено {approvedBudget}, а вартість завдання {cost}");
        }

        task.Complete();
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
