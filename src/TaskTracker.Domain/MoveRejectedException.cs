namespace TaskTracker.Domain;

/// <summary>Переміщення завдання відхилено профільним правилом.</summary>
public sealed class MoveRejectedException : DomainException
{
    /// <summary>Створює виняток зі стандартним поясненням.</summary>
    public MoveRejectedException()
        : this(MoveVerdict.ForbiddenTransition, "Переміщення відхилено.")
    {
    }

    /// <summary>Створює виняток із поясненням.</summary>
    /// <param name="message">Пояснення українською.</param>
    public MoveRejectedException(string message)
        : this(MoveVerdict.ForbiddenTransition, message)
    {
    }

    /// <summary>Створює виняток із поясненням і причиною.</summary>
    /// <param name="message">Пояснення українською.</param>
    /// <param name="innerException">Виняток, що спричинив відмову.</param>
    public MoveRejectedException(string message, Exception innerException)
        : base(message, innerException)
    {
        Verdict = MoveVerdict.ForbiddenTransition;
    }

    /// <summary>Створює виняток для конкретного рішення правила.</summary>
    /// <param name="verdict">Причина відмови; не <see cref="MoveVerdict.Allowed"/>.</param>
    /// <param name="message">Пояснення українською з номером завдання й колонками.</param>
    public MoveRejectedException(MoveVerdict verdict, string message)
        : base(message)
    {
        Verdict = verdict;
    }

    /// <summary>Причина відмови.</summary>
    public MoveVerdict Verdict { get; }
}
