using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Abstractions;

/// <summary>Сховище завдань: лише зберігання, жодних листів і звітів.</summary>
public interface ITaskRepository
{
    /// <summary>Додає нове завдання.</summary>
    /// <param name="task">Завдання з унікальним ідентифікатором.</param>
    void Add(WorkTask task);

    /// <summary>Шукає завдання за ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор завдання.</param>
    /// <returns>Завдання або null.</returns>
    WorkTask? GetById(int id);

    /// <summary>Повертає всі завдання.</summary>
    /// <returns>Знімок списку завдань.</returns>
    IReadOnlyList<WorkTask> GetAll();
}
