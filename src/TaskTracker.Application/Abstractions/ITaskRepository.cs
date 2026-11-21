using TaskTracker.Domain;

namespace TaskTracker.Application.Abstractions;

/// <summary>Сховище завдань дошки; формат зберігання прикладному шару невідомий.</summary>
public interface ITaskRepository
{
    /// <summary>Повертає всі завдання дошки.</summary>
    /// <returns>Знімок списку завдань.</returns>
    IReadOnlyList<BoardTask> GetAll();

    /// <summary>Шукає завдання за номером.</summary>
    /// <param name="id">Номер завдання.</param>
    /// <returns>Завдання або null, якщо такого номера немає.</returns>
    BoardTask? FindById(int id);

    /// <summary>Видає номер для нового завдання.</summary>
    /// <returns>Номер, більший за всі наявні.</returns>
    int NextId();

    /// <summary>Додає нове завдання.</summary>
    /// <param name="task">Завдання з унікальним номером.</param>
    void Add(BoardTask task);

    /// <summary>Зберігає всі зміни дошки.</summary>
    /// <exception cref="IOException">Сховище недоступне для запису.</exception>
    void SaveChanges();
}
