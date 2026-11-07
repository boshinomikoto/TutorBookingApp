namespace TutorBookingApp.Api.Models
{
    /// <summary>
    /// Статус запису на консультацію (відповідає State Machine Diagram, ЛР №3).
    /// Scheduled -> Completed | Cancelled — обидва кінцеві стани.
    /// </summary>
    public enum ConsultationStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}
