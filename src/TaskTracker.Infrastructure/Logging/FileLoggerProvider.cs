using System.Globalization;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Abstractions;

namespace TaskTracker.Infrastructure.Logging;

/// <summary>
/// Постачальник журналу у файл: рядок «час;рівень;сценарій;повідомлення».
/// Вміст файлів і персональні дані до журналу не пишуться.
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss";

    private readonly StreamWriter _writer;
    private readonly IClock _clock;
    private readonly ScenarioScope _scope = new();
    private readonly object _gate = new();

    /// <summary>Відкриває файл журналу для дописування.</summary>
    /// <param name="path">Шлях до файлу журналу.</param>
    /// <param name="clock">Годинник для міток часу.</param>
    public FileLoggerProvider(string path, IClock clock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        string? folder = Path.GetDirectoryName(Path.GetFullPath(path));
        if (folder is not null)
        {
            Directory.CreateDirectory(folder);
        }

        _writer = new StreamWriter(path, append: true) { AutoFlush = true };
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => new FileLogger(this);

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_gate)
        {
            _writer.Dispose();
        }
    }

    /// <summary>Поточна область сценарію для всіх журналів постачальника.</summary>
    /// <returns>Спільна область.</returns>
    internal ScenarioScope Scope() => _scope;

    /// <summary>Записує один рядок журналу.</summary>
    /// <param name="level">Рівень запису.</param>
    /// <param name="message">Повідомлення.</param>
    internal void Write(LogLevel level, string message)
    {
        string time = _clock.Now.ToString(TimeFormat, CultureInfo.InvariantCulture);
        string line = string.Join(';', time, level, _scope.Current ?? "-", message);
        lock (_gate)
        {
            _writer.WriteLine(line);
        }
    }
}
