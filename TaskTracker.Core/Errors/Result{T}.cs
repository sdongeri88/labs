namespace TaskTracker.Core.Errors;

/// <summary>Результат операції: або значення, або пояснення помилки.</summary>
/// <typeparam name="T">Тип значення.</typeparam>
public readonly record struct Result<T>
{
    /// <summary>Створює результат; викликається лише через фабрику <see cref="Result"/>.</summary>
    /// <param name="isSuccess">Ознака успіху.</param>
    /// <param name="value">Значення.</param>
    /// <param name="error">Пояснення помилки.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>Чи успішна операція.</summary>
    public bool IsSuccess { get; }

    /// <summary>Значення (лише для успішного результату).</summary>
    public T? Value { get; }

    /// <summary>Пояснення помилки (порожнє для успішного результату).</summary>
    public string Error { get; }
}
