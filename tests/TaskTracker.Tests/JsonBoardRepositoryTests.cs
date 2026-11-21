using TaskTracker.Domain;
using TaskTracker.Infrastructure.Storage;

namespace TaskTracker.Tests;

/// <summary>Сховище JSON (інфраструктура; поза межею вимірювання покриття).</summary>
public sealed class JsonBoardRepositoryTests : IDisposable
{
    private readonly string _folder =
        Path.Combine(Path.GetTempPath(), "tt-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public void Open_MissingFile_SeedsDemoBoardAndSavesIt()
    {
        string path = Path.Combine(_folder, "board.json");

        JsonBoardRepository repository = JsonBoardRepository.Open(path, new DateOnly(2026, 11, 20));

        Assert.True(File.Exists(path));
        Assert.Equal(60, repository.GetAll().Count);
    }

    [Fact]
    public void SaveChanges_ThenOpen_RestoresColumnAssigneeAndHistory()
    {
        string path = Path.Combine(_folder, "board.json");
        JsonBoardRepository first = JsonBoardRepository.Open(path, new DateOnly(2026, 11, 20));
        BoardTask task = first.FindById(31)!;
        var policy = new WipMovePolicy(new WipLimits(99, 99, 99, 99));
        task.MoveTo(BoardColumn.Todo, policy, first.GetAll(), FixedClock.Moment);
        first.SaveChanges();

        JsonBoardRepository second = JsonBoardRepository.Open(path, new DateOnly(2026, 11, 21));
        BoardTask restored = second.FindById(31)!;

        Assert.Equal(
            (BoardColumn.Todo, FixedClock.Moment), (restored.Column, restored.History[0].At));
    }

    [Fact]
    public void Open_CorruptFile_ThrowsInvalidData()
    {
        Directory.CreateDirectory(_folder);
        string path = Path.Combine(_folder, "board.json");
        File.WriteAllText(path, "{ не json");

        Assert.Throws<InvalidDataException>(
            () => JsonBoardRepository.Open(path, new DateOnly(2026, 11, 20)));
    }

    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }
}
