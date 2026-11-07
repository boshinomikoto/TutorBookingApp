namespace TutorBookingApp.Api.Contracts
{
    /// <summary>Запит на реєстрацію учня (UC-01). FullName обов'язкове.</summary>
    public sealed record RegisterStudentRequest(string FullName, string? Phone, string? Notes);

    /// <summary>Учень у відповіді сервера.</summary>
    public sealed record StudentDto(int Id, string FullName, string? Phone, string? Notes);
}
