using TaskTracker.Domain;

namespace TaskTracker.Infrastructure.Storage;

/// <summary>Перетворення між об'єктами домену і серіалізаційними записами.</summary>
internal static class BoardMapper
{
    /// <summary>Відновлює учасника з запису.</summary>
    /// <param name="record">Запис файлу.</param>
    /// <returns>Учасник.</returns>
    public static Member ToMember(MemberRecord record) =>
        new(record.Login, record.Name, record.WeeklyCapacityHours);

    /// <summary>Перетворює учасника на запис.</summary>
    /// <param name="member">Учасник.</param>
    /// <returns>Запис файлу.</returns>
    public static MemberRecord ToRecord(Member member) =>
        new(member.Login, member.Name, member.WeeklyCapacityHours);

    /// <summary>Відновлює завдання з запису разом зі станом та історією.</summary>
    /// <param name="record">Запис файлу.</param>
    /// <returns>Завдання.</returns>
    public static BoardTask ToTask(TaskRecord record)
    {
        var details = new TaskDetails(record.Title, record.Priority, record.EstimateHours);
        var task = new BoardTask(record.Id, details, record.CreatedOn);
        IEnumerable<TaskMove> history =
            record.History.Select(m => new TaskMove(m.From, m.To, m.At));
        task.Restore(record.Column, record.Assignee, history);
        return task;
    }

    /// <summary>Перетворює завдання на запис.</summary>
    /// <param name="task">Завдання.</param>
    /// <returns>Запис файлу.</returns>
    public static TaskRecord ToRecord(BoardTask task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Priority = task.Priority,
        EstimateHours = task.EstimateHours,
        CreatedOn = task.CreatedOn,
        Assignee = task.AssigneeLogin,
        Column = task.Column,
        History = task.History.Select(m => new MoveRecord(m.From, m.To, m.At)).ToList(),
    };
}
