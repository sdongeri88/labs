using Microsoft.Extensions.Logging;
using TaskTracker.Application.Abstractions;
using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Фейк: сховище завдань у пам'яті замість файлу JSON.</summary>
internal sealed class FakeTaskRepository : ITaskRepository
{
    private readonly List<BoardTask> _items = [];

    public FakeTaskRepository(params BoardTask[] tasks) => _items.AddRange(tasks);

    public int SaveCalls { get; private set; }

    public bool FailOnSave { get; init; }

    public IReadOnlyList<BoardTask> GetAll() => _items.ToList();

    public BoardTask? FindById(int id) => _items.Find(t => t.Id == id);

    public int NextId() => _items.Count == 0 ? 1 : _items.Max(t => t.Id) + 1;

    public void Add(BoardTask task) => _items.Add(task);

    public void SaveChanges()
    {
        if (FailOnSave)
        {
            throw new IOException("Диск недоступний");
        }

        SaveCalls++;
    }
}

/// <summary>Фейк: довідник учасників у пам'яті.</summary>
internal sealed class FakeMemberDirectory : IMemberDirectory
{
    private readonly List<Member> _items;

    public FakeMemberDirectory(params Member[] members) => _items = members.ToList();

    public IReadOnlyList<Member> GetAll() => _items;

    public Member? FindByLogin(string login) =>
        _items.Find(m => string.Equals(m.Login, login, StringComparison.OrdinalIgnoreCase));
}

/// <summary>Фіксований годинник замість системного часу.</summary>
internal sealed class FixedClock : IClock
{
    public static DateTimeOffset Moment { get; } =
        new(2026, 11, 20, 10, 30, 0, TimeSpan.FromHours(2));

    public DateTimeOffset Now => Moment;

    public DateOnly Today => DateOnly.FromDateTime(Moment.DateTime);
}

/// <summary>Шпигун журналу: запам'ятовує рівні записів, щоб тест перевірив логування.</summary>
internal sealed class SpyLogger<T> : ILogger<T>
{
    public List<LogLevel> Levels { get; } = [];

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) => Levels.Add(logLevel);
}

/// <summary>Будівник завдань у потрібній колонці для тестів правила.</summary>
internal static class Tasks
{
    private static int _nextId = 1000;

    public static BoardTask In(BoardColumn column, string? assignee = null, decimal hours = 4m)
    {
        var task = new BoardTask(
            Interlocked.Increment(ref _nextId),
            new TaskDetails("Завдання", TaskPriority.Normal, hours),
            new DateOnly(2026, 11, 1));
        task.Restore(column, assignee, []);
        return task;
    }

    public static string IdOf(BoardTask task) =>
        task.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static List<BoardTask> Many(int count, BoardColumn column, string? assignee = null) =>
        Enumerable.Range(0, count).Select(_ => In(column, assignee)).ToList();
}
