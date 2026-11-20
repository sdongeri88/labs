namespace TaskTracker.Core.Errors;

/// <summary>Порушено правило предметної області (очікувана для домену ситуація).</summary>
public class DomainRuleException : Exception
{
    /// <summary>Ідентифікатор правила, коли його не вказано.</summary>
    public const string UnknownRule = "domain";

    /// <summary>Створює виняток без пояснення (стандартний конструктор).</summary>
    public DomainRuleException()
        : this(UnknownRule, "Порушено правило предметної області")
    {
    }

    /// <summary>Створює виняток із повідомленням (стандартний конструктор).</summary>
    /// <param name="message">Повідомлення українською.</param>
    public DomainRuleException(string message)
        : this(UnknownRule, message)
    {
    }

    /// <summary>Створює виняток із повідомленням і причиною (стандартний конструктор).</summary>
    /// <param name="message">Повідомлення українською.</param>
    /// <param name="inner">Початковий виняток.</param>
    public DomainRuleException(string message, Exception inner)
        : this(UnknownRule, message, inner)
    {
    }

    /// <summary>Створює виняток для правила.</summary>
    /// <param name="rule">Ідентифікатор правила, напр. «task.editable».</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>Створює виняток для правила з причиною нижчого рівня.</summary>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="message">Повідомлення українською.</param>
    /// <param name="inner">Початковий виняток, що спричинив порушення.</param>
    public DomainRuleException(string rule, string message, Exception inner)
        : base(message, inner)
    {
        Rule = rule;
    }

    /// <summary>Ідентифікатор порушеного правила.</summary>
    public string Rule { get; }
}
