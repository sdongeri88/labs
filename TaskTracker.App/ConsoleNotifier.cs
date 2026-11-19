using TaskTracker.Core.Abstractions;

namespace TaskTracker.App;

/// <summary>Сповіщувач для консольного застосунку: друкує повідомлення замість листа.</summary>
internal sealed class ConsoleNotifier : INotifier
{
    /// <inheritdoc/>
    public void Notify(string recipient, string message)
    {
        Console.WriteLine($"[сповіщення для {recipient}] {message}");
    }
}
