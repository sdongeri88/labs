using TaskTracker.Application.Services;
using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Cli;

/// <summary>
/// Меню консолі: читає ввід, викликає сценарії прикладного шару й показує результат.
/// Розрахунків і правил тут немає.
/// </summary>
internal sealed class BoardConsole
{
    private readonly TaskService _tasks;
    private readonly MoveService _moves;
    private readonly WorkloadReportService _report;
    private readonly ConsoleIo _io;
    private readonly Dictionary<string, Action> _commands;

    /// <summary>Створює меню з готовими сервісами.</summary>
    /// <param name="tasks">Сервіс завдань.</param>
    /// <param name="moves">Сервіс переміщень.</param>
    /// <param name="report">Сервіс звіту.</param>
    /// <param name="io">Консоль.</param>
    public BoardConsole(
        TaskService tasks, MoveService moves, WorkloadReportService report, ConsoleIo io)
    {
        _tasks = tasks;
        _moves = moves;
        _report = report;
        _io = io;
        _commands = new Dictionary<string, Action>(StringComparer.Ordinal)
        {
            ["1"] = CreateTask,
            ["2"] = MoveTask,
            ["3"] = AssignTask,
            ["4"] = FindTasks,
            ["5"] = PrintReport,
            ["6"] = SaveBoard,
        };
    }

    private static string[] Menu { get; } =
    [
        "1 — створити завдання (UC-01)",
        "2 — перемістити завдання (UC-02)",
        "3 — призначити виконавця (UC-03)",
        "4 — знайти завдання (UC-04)",
        "5 — звіт «завантаження виконавців» (UC-05)",
        "6 — зберегти дошку (UC-06)",
        "0 — зберегти й вийти",
    ];

    /// <summary>Головний цикл меню до вибору «0» або кінця вводу.</summary>
    public void Run()
    {
        _io.Say("TaskTracker — дошка команди. Дані: data/board.json");
        while (true)
        {
            Array.ForEach(Menu, _io.Say);
            string? choice = _io.Ask("Ваш вибір");
            if (choice is null || choice.Trim() == "0")
            {
                SaveBoard();
                return;
            }

            Dispatch(choice.Trim());
        }
    }

    private void Dispatch(string choice)
    {
        if (!_commands.TryGetValue(choice, out Action? command))
        {
            _io.Say("Немає такого пункту меню, введіть число від 0 до 6.");
            return;
        }

        try
        {
            command();
        }
        catch (DomainException ex)
        {
            _io.Say("Відхилено: " + ex.Message);
        }
    }

    private void CreateTask()
    {
        while (true)
        {
            string? title = _io.Ask("Назва (порожньо — скасувати)");
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            var input = new CreateTaskInput(
                title, _io.Ask("Пріоритет 1–4"), _io.Ask("Оцінка, год"), _io.Ask("Виконавець"));
            if (Show(_tasks.Create(input), "Створено"))
            {
                return;
            }
        }
    }

    private void MoveTask() =>
        Show(_moves.Move(_io.Ask("Номер завдання"), _io.Ask("Колонка")), "Переміщено");

    private void AssignTask() =>
        Show(_tasks.Assign(_io.Ask("Номер завдання"), _io.Ask("Логін")), "Призначено");

    private void FindTasks()
    {
        var query = new TaskQuery(
            _io.Ask("Виконавець"), _io.Ask("Колонка"), _io.Ask("Фрагмент назви"));
        Result<IReadOnlyList<BoardTask>> found = _tasks.Find(query);
        if (!found.IsSuccess)
        {
            _io.Say("Помилка вводу: " + found.Error);
            return;
        }

        _io.Say("Знайдено: " + found.Value!.Count.ToString(TaskPrinter.Ukrainian));
        foreach (BoardTask task in found.Value)
        {
            _io.Say(TaskPrinter.Line(task));
        }
    }

    private void PrintReport()
    {
        foreach (string line in WorkloadPrinter.Lines(_report.Build()))
        {
            _io.Say(line);
        }
    }

    private void SaveBoard()
    {
        try
        {
            _tasks.Save();
            _io.Say("Дошку збережено.");
        }
        catch (IOException ex)
        {
            _io.Say("Не вдалося зберегти дошку: " + ex.Message);
        }
    }

    private bool Show(Result<BoardTask> result, string verb)
    {
        _io.Say(result.IsSuccess
            ? verb + ": " + TaskPrinter.Line(result.Value!)
            : "Помилка вводу: " + result.Error);
        return result.IsSuccess;
    }
}
