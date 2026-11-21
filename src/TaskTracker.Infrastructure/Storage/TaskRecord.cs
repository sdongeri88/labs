using TaskTracker.Domain;

namespace TaskTracker.Infrastructure.Storage;

/// <summary>Серіалізаційний запис завдання: домен не залежить від формату файлу.</summary>
internal sealed record TaskRecord
{
    /// <summary>Номер завдання.</summary>
    public int Id { get; init; }

    /// <summary>Назва.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Пріоритет.</summary>
    public TaskPriority Priority { get; init; }

    /// <summary>Оцінка, годин.</summary>
    public decimal EstimateHours { get; init; }

    /// <summary>Дата створення.</summary>
    public DateOnly CreatedOn { get; init; }

    /// <summary>Логін виконавця або null.</summary>
    public string? Assignee { get; init; }

    /// <summary>Колонка дошки.</summary>
    public BoardColumn Column { get; init; }

    /// <summary>Історія переміщень.</summary>
    public IReadOnlyList<MoveRecord> History { get; init; } = [];
}
