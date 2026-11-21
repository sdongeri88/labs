using TaskTracker.Domain;

namespace TaskTracker.Application.Validation;

/// <summary>Перевіряє ввід сценарію UC-01 і збирає змістові дані завдання.</summary>
public static class TaskInputValidator
{
    /// <summary>Перевіряє назву, пріоритет і оцінку; зупиняється на першій помилці.</summary>
    /// <param name="input">Сирий ввід користувача.</param>
    /// <returns>Змістові дані завдання або пояснення.</returns>
    public static Result<TaskDetails> Validate(CreateTaskInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Result<string> title = InputParser.Title(input.Title);
        if (!title.IsSuccess)
        {
            return Result.Fail<TaskDetails>(title.Error);
        }

        Result<TaskPriority> priority = InputParser.Priority(input.Priority);
        if (!priority.IsSuccess)
        {
            return Result.Fail<TaskDetails>(priority.Error);
        }

        Result<decimal> hours = InputParser.Hours(input.EstimateHours);
        return hours.IsSuccess
            ? Result.Ok(new TaskDetails(title.Value!, priority.Value, hours.Value))
            : Result.Fail<TaskDetails>(hours.Error);
    }
}
