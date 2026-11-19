namespace TaskTracker.Core.Abstractions;

/// <summary>Сповіщувач: одна відповідальність — доставити повідомлення.</summary>
public interface INotifier
{
    /// <summary>Надсилає повідомлення адресатові.</summary>
    /// <param name="recipient">Адресат.</param>
    /// <param name="message">Текст повідомлення.</param>
    void Notify(string recipient, string message);
}
