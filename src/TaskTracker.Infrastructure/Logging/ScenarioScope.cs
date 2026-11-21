namespace TaskTracker.Infrastructure.Logging;

/// <summary>Зберігає ідентифікатор поточного сценарію (UC-xx) для рядків журналу.</summary>
internal sealed class ScenarioScope
{
    private readonly AsyncLocal<string?> _current = new();

    /// <summary>Ідентифікатор поточного сценарію або null поза сценарієм.</summary>
    public string? Current => _current.Value;

    /// <summary>Відкриває область сценарію; закриття повертає попередню.</summary>
    /// <param name="scenario">Ідентифікатор сценарію.</param>
    /// <returns>Об'єкт, що закриває область.</returns>
    public IDisposable Push(string? scenario)
    {
        string? previous = _current.Value;
        _current.Value = scenario;
        return new Restorer(this, previous);
    }

    /// <summary>Відновлює попередню область під час закриття.</summary>
    private sealed class Restorer : IDisposable
    {
        private readonly ScenarioScope _owner;
        private readonly string? _previous;

        /// <summary>Запам'ятовує область для відновлення.</summary>
        /// <param name="owner">Область сценаріїв.</param>
        /// <param name="previous">Попередній сценарій.</param>
        public Restorer(ScenarioScope owner, string? previous)
        {
            _owner = owner;
            _previous = previous;
        }

        /// <inheritdoc/>
        public void Dispose() => _owner._current.Value = _previous;
    }
}
