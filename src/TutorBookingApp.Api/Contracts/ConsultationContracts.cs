using TutorBookingApp.Api.Models;

namespace TutorBookingApp.Api.Contracts
{
    /// <summary>Запит на створення запису (UC-03). Перетин часу перевіряє сервер (FR-04).</summary>
    public sealed record CreateConsultationRequest(
        int StudentId, DateTime StartDateTime, int DurationMinutes, string? Topic, string? Notes);

    /// <summary>Запит на завершення запису (UC-05). Topic обов'язкове (FR-06).</summary>
    public sealed record CompleteConsultationRequest(string Topic);

    /// <summary>Запис на консультацію у відповіді сервера.</summary>
    public sealed record ConsultationDto(
        int Id, int StudentId, DateTime StartDateTime, int DurationMinutes,
        ConsultationStatus Status, string? Topic, string? Notes);
}
