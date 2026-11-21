namespace TaskTracker.Domain;

/// <summary>
/// Ліміти незавершеної роботи (WIP): на колонку і на одного виконавця.
/// Колонки Backlog і Done не обмежуються.
/// </summary>
public sealed class WipLimits
{
    /// <summary>Типовий ліміт колонки Todo.</summary>
    public const int DefaultTodo = 8;

    /// <summary>Типовий ліміт колонки InProgress.</summary>
    public const int DefaultInProgress = 5;

    /// <summary>Типовий ліміт колонки Review.</summary>
    public const int DefaultReview = 3;

    /// <summary>Типовий особистий ліміт активних завдань на людину.</summary>
    public const int DefaultPersonal = 2;

    /// <summary>Створює набір лімітів; кожен ліміт більший за нуль.</summary>
    /// <param name="todo">Ліміт колонки Todo.</param>
    /// <param name="inProgress">Ліміт колонки InProgress.</param>
    /// <param name="review">Ліміт колонки Review.</param>
    /// <param name="personal">Особистий ліміт активних завдань.</param>
    /// <exception cref="ArgumentOutOfRangeException">Будь-який ліміт не додатний.</exception>
    public WipLimits(int todo, int inProgress, int review, int personal)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(todo);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(inProgress);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(review);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(personal);

        Todo = todo;
        InProgress = inProgress;
        Review = review;
        Personal = personal;
    }

    /// <summary>Ліміти команди за замовчуванням.</summary>
    public static WipLimits Default { get; } =
        new(DefaultTodo, DefaultInProgress, DefaultReview, DefaultPersonal);

    /// <summary>Ліміт колонки Todo.</summary>
    public int Todo { get; }

    /// <summary>Ліміт колонки InProgress.</summary>
    public int InProgress { get; }

    /// <summary>Ліміт колонки Review.</summary>
    public int Review { get; }

    /// <summary>Скільки активних завдань може одночасно мати один виконавець.</summary>
    public int Personal { get; }

    /// <summary>Повертає ліміт колонки або null, якщо колонка не обмежена.</summary>
    /// <param name="column">Колонка дошки.</param>
    /// <returns>Ліміт або null для Backlog і Done.</returns>
    public int? ForColumn(BoardColumn column) => column switch
    {
        BoardColumn.Todo => Todo,
        BoardColumn.InProgress => InProgress,
        BoardColumn.Review => Review,
        BoardColumn.Backlog or BoardColumn.Done => null,
        _ => throw new ArgumentOutOfRangeException(nameof(column), column, "Невідома колонка."),
    };
}
