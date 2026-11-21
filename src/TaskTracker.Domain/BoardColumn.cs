namespace TaskTracker.Domain;

/// <summary>Колонка дошки, у якій перебуває завдання (стан документа).</summary>
public enum BoardColumn
{
    /// <summary>Беклог: завдання записано, але не заплановано.</summary>
        Backlog  =  0,

    /// <summary>Заплановано на поточну ітерацію.</summary>
    Todo = 1,

    /// <summary>Завдання в роботі у виконавця.</summary>
    InProgress = 2,

    /// <summary>Завдання на перевірці.</summary>
    Review = 3,

    /// <summary>Завдання виконано; кінцевий стан.</summary>
    Done = 4,
}
