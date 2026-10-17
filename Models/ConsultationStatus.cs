namespace TutorBookingApp.Models
{
    /// <summary>
    /// Статус запису на консультацію (відповідає State Machine Diagram у docs/design.md).
    /// Scheduled -> Completed | Cancelled — обидва кінцеві стани, назад у Scheduled не повертаються.
    /// </summary>
    public enum ConsultationStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}
