using TaskTracker.Domain;

namespace TaskTracker.Application.Services;

/// <summary>Звіт «завантаження виконавців» (UC-05).</summary>
/// <param name="Rows">Рядки за спаданням завантаження, потім за логіном.</param>
/// <param name="TeamLoadPercent">Завантаження команди загалом, %.</param>
/// <param name="BuiltOn">Дата побудови звіту.</param>
public sealed record WorkloadReport(
    IReadOnlyList<MemberWorkload> Rows, decimal TeamLoadPercent, DateOnly BuiltOn);
