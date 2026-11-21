namespace TaskTracker.Domain;

/// <summary>Рішення профільного правила щодо переміщення завдання.</summary>
public enum MoveVerdict
{
    /// <summary>Переміщення дозволене.</summary>
    Allowed = 0,

    /// <summary>Переходу немає в таблиці переходів.</summary>
    ForbiddenTransition = 1,

    /// <summary>У роботу не можна взяти завдання без виконавця.</summary>
    AssigneeRequired = 2,

    /// <summary>Цільова колонка вже заповнена до свого ліміту WIP.</summary>
    ColumnLimitReached = 3,

    /// <summary>Виконавець уже має максимум активних завдань.</summary>
    PersonalLimitReached = 4,
}
