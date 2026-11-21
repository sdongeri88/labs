namespace TaskTracker.Domain;

/// <summary>Порушено правило предметної області; очікувана ситуація, а не збій.</summary>
public class DomainException : Exception
{
    /// <summary>Створює виняток зі стандартним поясненням.</summary>
    public DomainException()
        : base("Порушено правило предметної області.")
    {
    }

    /// <summary>Створює виняток із поясненням.</summary>
    /// <param name="message">Пояснення українською з конкретними значеннями.</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>Створює виняток із поясненням і причиною.</summary>
    /// <param name="message">Пояснення українською.</param>
    /// <param name="innerException">Виняток, що спричинив порушення.</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
