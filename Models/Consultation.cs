using System;

namespace TutorBookingApp.Models
{
    /// <summary>
    /// Запис на консультацію (відповідає Class Diagram у docs/design.md).
    /// Один Student може мати багато Consultation (1 : 0..*).
    /// Перевірка перетину часу (FR-04) і решта бізнес-правил — у ConsultationService,
    /// який буде додано на наступному кроці (ЛР №4, docs/implementation-plan.md).
    /// </summary>
    public class Consultation
    {
        public int Id { get; set; }

        // Зовнішній ключ на Student.
        public int StudentId { get; set; }

        public DateTime StartDateTime { get; set; }
        public int DurationMinutes { get; set; }

        // Новий запис завжди створюється як Scheduled (UC-03).
        public ConsultationStatus Status { get; set; } = ConsultationStatus.Scheduled;

        // Topic обов'язковий лише в момент переходу в Completed (FR-06, AC-06),
        // тому тут залишається необов'язковим полем.
        public string? Topic { get; set; }
        public string? Notes { get; set; }
    }
}
