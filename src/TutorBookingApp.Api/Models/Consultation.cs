namespace TutorBookingApp.Api.Models
{
    /// <summary>
    /// Запис на консультацію (Class Diagram, ЛР №3). Один Student має багато
    /// Consultation (1 : 0..*). Перевірка перетину часу (FR-04) — у Program.cs,
    /// обробник POST /api/consultations (UC-03).
    /// </summary>
    public sealed class Consultation
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int DurationMinutes { get; set; }

        // Новий запис завжди створюється як Scheduled (UC-03).
        public ConsultationStatus Status { get; set; } = ConsultationStatus.Scheduled;

        // Topic обов'язковий лише в момент переходу в Completed (FR-06, AC-06).
        public string? Topic { get; set; }
        public string? Notes { get; set; }
    }
}
