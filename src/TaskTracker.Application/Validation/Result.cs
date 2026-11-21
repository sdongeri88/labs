namespace TaskTracker.Application.Validation;

/// <summary>Фабрика результатів перевірки.</summary>
public static class Result
{
    /// <summary>Успішний результат зі значенням.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення.</param>
    /// <returns>Успішний результат.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, string.Empty);

    /// <summary>Невдалий результат із поясненням.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="error">Пояснення для користувача, не порожнє.</param>
    /// <returns>Невдалий результат.</returns>
    public static Result<T> Fail<T>(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new(false, default, error);
    }
}
