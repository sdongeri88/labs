namespace TaskTracker.Core.Requests;

/// <summary>Стан заявки на виконання робіт.</summary>
public enum RequestStatus
{
    /// <summary>Заявку створено, вона ще не погоджена.</summary>
    New,

    /// <summary>Заявку погоджено й оплачено.</summary>
    Approved,

    /// <summary>Роботи за заявкою виконано.</summary>
    Completed,

    /// <summary>Заявку скасовано.</summary>
    Cancelled,
}
