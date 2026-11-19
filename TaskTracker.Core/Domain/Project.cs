namespace TaskTracker.Core.Domain;

/// <summary>Проєкт — об’єкт обліку; змінна лише кількість відкритих завдань.</summary>
public sealed class Project
{
    /// <summary>Створює проєкт без відкритих завдань.</summary>
    /// <param name="code">Код проєкту, не порожній.</param>
    /// <param name="title">Назва, не порожня.</param>
    /// <param name="hourlyRate">Погодинна ставка, більша за нуль.</param>
    public Project(string code, string title, decimal hourlyRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hourlyRate);

        Code = code;
        Title = title;
        HourlyRate = hourlyRate;
    }

    /// <summary>Код проєкту.</summary>
    public string Code { get; }

    /// <summary>Назва проєкту.</summary>
    public string Title { get; }

    /// <summary>Погодинна ставка проєкту, грн.</summary>
    public decimal HourlyRate { get; }

    /// <summary>Кількість відкритих завдань, завжди не від’ємна.</summary>
    public int OpenTasks { get; private set; }

    /// <summary>Реєструє нове відкрите завдання.</summary>
    public void OpenTask() => OpenTasks++;

    /// <summary>Закриває одне відкрите завдання.</summary>
    public void CloseTask()
    {
        if (OpenTasks == 0)
        {
            throw new InvalidOperationException("У проєкті немає відкритих завдань.");
        }

        OpenTasks--;
    }
}
