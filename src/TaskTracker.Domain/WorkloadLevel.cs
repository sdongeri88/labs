namespace TaskTracker.Domain;

/// <summary>Рівень завантаження виконавця для звіту.</summary>
public enum WorkloadLevel
{
    /// <summary>Завантаження нижче за поріг «нормально».</summary>
    Free = 0,

    /// <summary>Завантаження в межах місткості.</summary>
    Normal = 1,

    /// <summary>Завантаження перевищує місткість.</summary>
    Overloaded = 2,
}
