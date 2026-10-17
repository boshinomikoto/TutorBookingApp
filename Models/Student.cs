namespace TutorBookingApp.Models
{
    /// <summary>
    /// Учень (відповідає Class Diagram у docs/design.md).
    /// Не має власного життєвого циклу зі станами — дані редагуються без обмежень,
    /// тому окремої State Machine Diagram для Student немає (обґрунтовано в docs/design.md).
    /// </summary>
    public class Student
    {
        public int Id { get; set; }

        // Обов'язкове поле (FR-01, AC-01).
        public string FullName { get; set; } = string.Empty;

        // Необов'язкові поля.
        public string? Phone { get; set; }
        public string? Notes { get; set; }
    }
}
