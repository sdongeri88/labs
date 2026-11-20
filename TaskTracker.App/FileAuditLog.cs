using System.Globalization;

namespace TaskTracker.App;

/// <summary>Журнал аудиту у файлі: дописує рядок «час;операція;документ».</summary>
internal sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Відкриває файл аудиту для дописування.</summary>
    /// <param name="path">Шлях до файлу.</param>
    public FileAuditLog(string path)
    {
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Дописує рядок аудиту.</summary>
    /// <param name="operation">Операція.</param>
    /// <param name="documentId">Ідентифікатор документа.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer.WriteLine(
            string.Create(
                CultureInfo.InvariantCulture, $"{DateTimeOffset.Now:O};{operation};{documentId}"));
    }

    /// <summary>Скидає буфер і закриває файл; повторний виклик безпечний.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
