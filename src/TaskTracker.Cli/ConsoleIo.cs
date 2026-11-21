namespace TaskTracker.Cli;

/// <summary>Читання і запис консолі; під час перенаправлення вводу повторює введене.</summary>
internal sealed class ConsoleIo
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly bool _echo;

    /// <summary>Створює обгортку над потоками консолі.</summary>
    /// <param name="input">Потік вводу.</param>
    /// <param name="output">Потік виводу.</param>
    /// <param name="echo">Повторювати введені рядки (для запису сеансу у файл).</param>
    public ConsoleIo(TextReader input, TextWriter output, bool echo)
    {
        _input = input;
        _output = output;
        _echo = echo;
    }

    /// <summary>Показує запрошення й читає рядок.</summary>
    /// <param name="prompt">Запрошення.</param>
    /// <returns>Введений рядок або null наприкінці вводу.</returns>
    public string? Ask(string prompt)
    {
        _output.Write(prompt + ": ");
        string? line = _input.ReadLine();
        if (_echo)
        {
            _output.WriteLine(line);
        }

        return line;
    }

    /// <summary>Друкує рядок.</summary>
    /// <param name="text">Текст.</param>
    public void Say(string text) => _output.WriteLine(text);
}
