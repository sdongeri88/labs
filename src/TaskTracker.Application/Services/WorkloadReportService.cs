using TaskTracker.Application.Abstractions;
using TaskTracker.Domain;

namespace TaskTracker.Application.Services;

/// <summary>Сценарій UC-05: звіт «завантаження виконавців».</summary>
public sealed class WorkloadReportService
{
    private readonly ITaskRepository _tasks;
    private readonly IMemberDirectory _members;
    private readonly IClock _clock;

    /// <summary>Створює сервіс звіту.</summary>
    /// <param name="tasks">Сховище завдань.</param>
    /// <param name="members">Довідник учасників.</param>
    /// <param name="clock">Годинник для дати звіту.</param>
    public WorkloadReportService(ITaskRepository tasks, IMemberDirectory members, IClock clock)
    {
        _tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        _members = members ?? throw new ArgumentNullException(nameof(members));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <summary>Будує звіт для всіх учасників команди.</summary>
    /// <returns>Звіт; для команди без учасників — порожні рядки й 0 %.</returns>
    public WorkloadReport Build()
    {
        IReadOnlyList<BoardTask> board = _tasks.GetAll();
        List<MemberWorkload> rows = _members.GetAll()
            .Select(m => WorkloadCalculator.Calculate(m, board))
            .OrderByDescending(r => r.LoadPercent)
            .ThenBy(r => r.Login, StringComparer.Ordinal)
            .ToList();

        decimal capacity = rows.Sum(r => r.CapacityHours);
        decimal team = capacity == 0m
            ? 0m
            : WorkloadCalculator.LoadPercent(rows.Sum(r => r.ActiveHours), capacity);
        return new WorkloadReport(rows, team, _clock.Today);
    }
}
