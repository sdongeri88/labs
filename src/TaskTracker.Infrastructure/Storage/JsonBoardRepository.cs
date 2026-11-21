using System.Text.Json;
using System.Text.Json.Serialization;
using TaskTracker.Application.Abstractions;
using TaskTracker.Domain;
using TaskTracker.Infrastructure.Seed;

namespace TaskTracker.Infrastructure.Storage;

/// <summary>Сховище дошки в одному файлі JSON: учасники й завдання з історією.</summary>
public sealed class JsonBoardRepository : ITaskRepository, IMemberDirectory
{
    private readonly string _path;
    private readonly List<Member> _members;
    private readonly List<BoardTask> _tasks;

    private JsonBoardRepository(
        string path, IEnumerable<Member> members, IEnumerable<BoardTask> tasks)
    {
        _path = path;
        _members = members.ToList();
        _tasks = tasks.ToList();
    }

    private static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>Відкриває файл дошки; без файлу створює його з умовних даних.</summary>
    /// <param name="path">Шлях до файлу JSON.</param>
    /// <param name="today">Дата для умовних даних.</param>
    /// <returns>Сховище з даними файлу.</returns>
    /// <exception cref="InvalidDataException">Файл існує, але не є дошкою.</exception>
    public static JsonBoardRepository Open(string path, DateOnly today)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path))
        {
            DemoBoard seed = DemoBoardFactory.Create(today);
            var created = new JsonBoardRepository(path, seed.Members, seed.Tasks);
            created.SaveChanges();
            return created;
        }

        BoardDocument document = Read(path);
        return new JsonBoardRepository(
            path,
            document.Members.Select(BoardMapper.ToMember),
            document.Tasks.Select(BoardMapper.ToTask));
    }

    /// <inheritdoc/>
    public IReadOnlyList<BoardTask> GetAll() => _tasks.AsReadOnly();

    /// <inheritdoc/>
    public BoardTask? FindById(int id) => _tasks.Find(t => t.Id == id);

    /// <inheritdoc/>
    public int NextId() => _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;

    /// <inheritdoc/>
    public void Add(BoardTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        _tasks.Add(task);
    }

    /// <inheritdoc/>
    public void SaveChanges()
    {
        var document = new BoardDocument
        {
            Members = _members.Select(BoardMapper.ToRecord).ToList(),
            Tasks = _tasks.Select(BoardMapper.ToRecord).ToList(),
        };
        string? folder = Path.GetDirectoryName(Path.GetFullPath(_path));
        if (folder is not null)
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(_path, JsonSerializer.Serialize(document, Options));
    }

    /// <inheritdoc/>
    IReadOnlyList<Member> IMemberDirectory.GetAll() => _members.AsReadOnly();

    /// <inheritdoc/>
    public Member? FindByLogin(string login)
    {
        ArgumentNullException.ThrowIfNull(login);
        return _members.Find(
            m => string.Equals(m.Login, login.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static BoardDocument Read(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<BoardDocument>(File.ReadAllText(path), Options)
                ?? throw new InvalidDataException($"Файл {path} порожній.");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Файл {path} не є файлом дошки.", ex);
        }
    }
}
