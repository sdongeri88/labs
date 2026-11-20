using Microsoft.Extensions.Logging;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс завдань: сценарії реєстрації, обліку часу й закриття.</summary>
public sealed partial class WorkTaskService
{
    /// <summary>Адресат службових сповіщень про нові завдання.</summary>
    public const string LeadRecipient = "team-lead";

    /// <summary>Записи, довші за цю кількість годин, підозрілі (Warning).</summary>
    public const decimal LongEntryHours = 12m;

    private readonly ITaskRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<WorkTaskService> _log;

    /// <summary>Створює сервіс із залежностями, переданими ззовні.</summary>
    /// <param name="repository">Сховище завдань.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    /// <param name="notifier">Сповіщувач.</param>
    /// <param name="log">Журнал сервісу.</param>
    public WorkTaskService(
        ITaskRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<WorkTaskService> log)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>Реєструє завдання у сховищі й сповіщає керівника.</summary>
    /// <param name="task">Завдання.</param>
    public void Register(WorkTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        _repository.Add(task);
        _notifier.Notify(LeadRecipient, $"Завдання #{task.Id} ({task.ProjectCode}) зареєстровано");
        _log.LogInformation(
            "Завдання {TaskId} зареєстровано в проєкті {ProjectCode}", task.Id, task.ProjectCode);
    }

    /// <summary>Повертає завдання або кидає виняток, якщо його немає.</summary>
    /// <param name="taskId">Ідентифікатор завдання.</param>
    /// <returns>Знайдене завдання.</returns>
    public WorkTask Load(int taskId) =>
        _repository.GetById(taskId)
        ?? throw new DomainRuleException("task.exists", $"Завдання {taskId} не знайдено");

    /// <summary>Додає запис часу до завдання, що перебуває в роботі.</summary>
    /// <param name="taskId">Ідентифікатор завдання.</param>
    /// <param name="log">Перевірений запис часу.</param>
    public void AddLog(int taskId, WorkLog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        using IDisposable? scope = _log.BeginScope("Task:{TaskId}", taskId);
        _log.LogDebug(
            "Додавання запису {Activity}: {Hours} год × {Rate}",
            log.Activity,
            log.Hours,
            log.HourlyRate);
        WorkTask task = _repository.GetById(taskId)
            ?? throw new DomainRuleException("task.exists", $"Завдання {taskId} не знайдено");
        if (task.Status != WorkTaskStatus.InProgress)
        {
            throw new DomainRuleException(
                "task.editable", $"Стан {task.Status} не дозволяє облік часу завдання {taskId}");
        }

        if (log.Hours > LongEntryHours)
        {
            _log.LogWarning("Довгий запис: {Hours} год за один раз", log.Hours);
        }

        task.AddLog(log);
        _log.LogInformation("Запис додано, разом {TotalHours} год", task.TotalHours);
    }

    /// <summary>Закриває перевірене завдання в межах погодженого бюджету.</summary>
    /// <param name="taskId">Ідентифікатор завдання.</param>
    /// <param name="approvedBudget">Погоджений бюджет, грн.</param>
    public void CloseTask(int taskId, decimal approvedBudget)
    {
        using IDisposable? scope = _log.BeginScope("Task:{TaskId}", taskId);
        _log.LogDebug("Закриття, погоджений бюджет {Budget}", approvedBudget);
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
        _log.LogInformation("Завдання закрито, вартість {Cost}", cost);
    }

    /// <summary>Скасовує завдання; заборонений перехід стає порушенням правила.</summary>
    /// <param name="taskId">Ідентифікатор завдання.</param>
    /// <param name="reason">Причина скасування.</param>
    public void Cancel(int taskId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        using IDisposable? scope = _log.BeginScope("Task:{TaskId}", taskId);
        WorkTask task = Load(taskId);
        try
        {
            task.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            _log.LogError(ex, "Скасування відхилено, причина запиту: {Reason}", reason);
            throw new DomainRuleException(
                "task.cancellable",
                $"Завдання {taskId} у стані {task.Status} не можна скасувати",
                ex);
        }

        _log.LogInformation("Завдання скасовано: {Reason}", reason);
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
