using TaskTracker.Domain;

namespace TaskTracker.Infrastructure.Seed;

/// <summary>
/// Генерує умовні дані: 8 вигаданих учасників і 60 завдань, розкладених так,
/// щоб не порушувати ліміти WIP. Реальних персональних даних немає.
/// </summary>
public static class DemoBoardFactory
{
    /// <summary>Кількість завдань в умовних даних.</summary>
    public const int TaskCount = 60;

    private const int DaysSpread = 30;
    private const int HoursStep = 4;
    private const int HoursVariants = 6;

    private static string[] Verbs { get; } =
        ["Додати", "Виправити", "Оновити", "Перевірити", "Описати", "Прискорити"];

    private static string[] Subjects { get; } =
    [
        "експорт звіту в CSV", "форму входу", "фільтр завдань", "сповіщення команди",
        "журнал змін", "імпорт учасників", "історію переміщень", "налаштування дошки",
        "пошук за тегом", "резервну копію",
    ];

    private static Member[] Team { get; } =
    [
        new("ivan", "Іван Мороз", 30m), new("olena", "Олена Кравець", 10m),
        new("taras", "Тарас Гнатюк", 40m), new("sofia", "Софія Литвин", 30m),
        new("petro", "Петро Савчук", 25m), new("maria", "Марія Ткач", 35m),
        new("andrii", "Андрій Бойко", 30m), new("nadia", "Надія Бондар", 20m),
    ];

    /// <summary>Створює умовну дошку на задану дату.</summary>
    /// <param name="today">Дата, від якої відлічуються дати створення.</param>
    /// <returns>Учасники й завдання.</returns>
    public static DemoBoard Create(DateOnly today)
    {
        List<BoardTask> tasks = [];
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(CreateTask(i, today));
        }

        return new DemoBoard(Team, tasks);
    }

    private static BoardTask CreateTask(int index, DateOnly today)
    {
        string title = Verbs[index % Verbs.Length] + " " + Subjects[index % Subjects.Length];
        var priority = (TaskPriority)((index % 4) + 1);
        decimal hours = HoursStep * ((index % HoursVariants) + 1);
        DateOnly createdOn = today.AddDays(-(index % DaysSpread));
        var task = new BoardTask(index + 1, new TaskDetails(title, priority, hours), createdOn);
        (BoardColumn column, string? assignee) = Placement(index);
        task.Restore(column, assignee, []);
        return task;
    }

    private static (BoardColumn Column, string? Assignee) Placement(int index) => index switch
    {
        < 5 => (BoardColumn.InProgress, Team[index].Login),
        < 8 => (BoardColumn.Review, Team[index - 5].Login),
        < 15 => (BoardColumn.Todo, index % 2 == 0 ? Team[index % Team.Length].Login : null),
        < 30 => (BoardColumn.Done, Team[index % Team.Length].Login),
        _ => (BoardColumn.Backlog, null),
    };
}
