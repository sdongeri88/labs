namespace TaskTracker.Application.Services;

/// <summary>Шаблони записів журналу прикладного шару (сталі, щоб журнал був однаковим).</summary>
internal static class LogText
{
    /// <summary>Шаблон області журналу: ідентифікатор сценарію.</summary>
    public const string Scope = "{Scenario}";

    /// <summary>Початок сценарію (Information).</summary>
    public const string Started = "Сценарій розпочато";

    /// <summary>Успішне завершення сценарію з номером завдання (Information).</summary>
    public const string Finished = "Сценарій завершено, завдання #{TaskId}";

    /// <summary>Дошку збережено (Information).</summary>
    public const string Saved = "Дошку збережено, завдань: {Count}";

    /// <summary>Відхилена операція (Warning).</summary>
    public const string Rejected = "Операцію відхилено: {Reason}";

    /// <summary>Збій сховища (Error).</summary>
    public const string StorageFailed = "Збій збереження дошки, завдань: {Count}";
}
