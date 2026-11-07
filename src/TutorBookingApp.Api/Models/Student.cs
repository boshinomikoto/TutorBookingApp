namespace TutorBookingApp.Api.Models
{
    /// <summary>
    /// Учень (Class Diagram, ЛР №3). Життєвого циклу зі станами не має —
    /// дані редагуються без обмежень, тому власної State Machine Diagram нема.
    /// </summary>
    public sealed class Student
    {
        public int Id { get; set; }

        // Обов'язкове поле (FR-01, AC-01).
        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string? Notes { get; set; }
    }
}
