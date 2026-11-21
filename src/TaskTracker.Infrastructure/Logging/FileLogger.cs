using Microsoft.Extensions.Logging;

namespace TaskTracker.Infrastructure.Logging;

/// <summary>Журнал однієї категорії; пише через спільний постачальник.</summary>
internal sealed class FileLogger : ILogger
{
    private readonly FileLoggerProvider _provider;

    /// <summary>Створює журнал, прив'язаний до постачальника.</summary>
    /// <param name="provider">Постачальник із відкритим файлом.</param>
    public FileLogger(FileLoggerProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        string? scenario = state is IReadOnlyList<KeyValuePair<string, object?>> pairs
            ? pairs.FirstOrDefault(p => p.Key == "Scenario").Value?.ToString()
            : null;
        return _provider.Scope().Push(scenario);
    }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    /// <inheritdoc/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);
        if (exception is not null)
        {
            message += " | " + exception.GetType().Name + ": " + exception.Message;
        }

        _provider.Write(logLevel, message);
    }
}
