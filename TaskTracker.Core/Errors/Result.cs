namespace TaskTracker.Core.Errors;

/// <summary>Фабрика результатів (статичні методи винесено з generic-типу, CA1000).</summary>
public static class Result
{
    /// <summary>Успішний результат.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення.</param>
    /// <returns>Результат зі значенням і порожнім поясненням.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, string.Empty);

    /// <summary>Невдалий результат із поясненням.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="error">Пояснення для користувача.</param>
    /// <returns>Результат без значення з поясненням помилки.</returns>
    public static Result<T> Fail<T>(string error) => new(false, default, error);
}
