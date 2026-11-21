namespace TaskTracker.Domain;

/// <summary>Правило, що вирішує, чи можна перемістити завдання до колонки.</summary>
public interface IMovePolicy
{
    /// <summary>Оцінює переміщення без побічних ефектів.</summary>
    /// <param name="task">Завдання, яке переміщують.</param>
    /// <param name="target">Цільова колонка.</param>
    /// <param name="board">Усі завдання дошки, зокрема саме завдання.</param>
    /// <returns>Рішення; <see cref="MoveVerdict.Allowed"/>, якщо перешкод немає.</returns>
    MoveVerdict Check(BoardTask task, BoardColumn target, IReadOnlyCollection<BoardTask> board);
}
