using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

public sealed partial class WorkTaskService
{
    // АНТИПАТЕРН 1: порожній catch, дефект зникає
    public WorkTask? Load(int taskId)
    {
        try
        {
            return _repository.GetById(taskId);
        }
        catch (Exception)
        {
        }
        return null;
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення, throw ex
    public void Cancel(int taskId, string reason)
    {
        try
        {
            var task = _repository.GetById(taskId);
            task.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    // АНТИПАТЕРН 4: try/catch замість звичайної умови
    public int CountValidHours(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var h = int.Parse(row);
                if (h > 0) valid++;
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }
        return valid;
    }
}
