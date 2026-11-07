using System.Text.Json.Serialization;
using TutorBookingApp.Api.Contracts;
using TutorBookingApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Робочі дані зберігаються лише в пам'яті серверного процесу (п.4.4.2 ЛР №4):
// після перезапуску сервера всі записи втрачаються.
var gate = new object();
var nextStudentId = 1;
var nextConsultationId = 1;
var students = new List<Student>();
var consultations = new List<Consultation>();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Спільне читання: клієнту потрібен список учнів і записів,
// щоб запропонувати їх при створенні/завершенні запису.
app.MapGet("/api/students", () =>
{
    lock (gate) return Results.Ok(students.Select(ToStudentDto).ToArray());
});

app.MapGet("/api/consultations", () =>
{
    lock (gate) return Results.Ok(consultations.Select(ToConsultationDto).ToArray());
});

// UC-01 (Максим) — зареєструвати учня. FR-01, AC-01.
app.MapPost("/api/students", (RegisterStudentRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.FullName))
        return Results.BadRequest(new ApiError("full_name_required", "Вкажіть ім'я учня."));

    lock (gate)
    {
        var student = new Student
        {
            Id = nextStudentId++,
            FullName = request.FullName.Trim(),
            Phone = request.Phone,
            Notes = request.Notes,
        };
        students.Add(student);
        return Results.Created($"/api/students/{student.Id}", ToStudentDto(student));
    }
});

// UC-03 (Вадим) — створити запис на консультацію. FR-03, FR-04, AC-03, AC-04.
app.MapPost("/api/consultations", (CreateConsultationRequest request) =>
{
    if (request.DurationMinutes <= 0)
        return Results.BadRequest(new ApiError("invalid_duration", "Тривалість має бути більшою за нуль."));

    lock (gate)
    {
        if (!students.Any(s => s.Id == request.StudentId))
            return Results.BadRequest(new ApiError("student_not_found", "Учня не знайдено."));

        var newStart = request.StartDateTime;
        var newEnd = request.StartDateTime.AddMinutes(request.DurationMinutes);

        // FR-04: перетин перевіряється лише серед записів зі статусом Scheduled —
        // скасований запис звільняє час (State Machine Diagram, ЛР №3).
        var overlaps = consultations.Any(c =>
            c.Status == ConsultationStatus.Scheduled &&
            newStart < c.StartDateTime.AddMinutes(c.DurationMinutes) &&
            c.StartDateTime < newEnd);

        if (overlaps)
            return Results.Conflict(new ApiError("schedule_conflict", "Час перетинається з іншим активним записом."));

        var consultation = new Consultation
        {
            Id = nextConsultationId++,
            StudentId = request.StudentId,
            StartDateTime = request.StartDateTime,
            DurationMinutes = request.DurationMinutes,
            Status = ConsultationStatus.Scheduled,
            Topic = request.Topic,
            Notes = request.Notes,
        };
        consultations.Add(consultation);
        return Results.Created($"/api/consultations/{consultation.Id}", ToConsultationDto(consultation));
    }
});

// UC-05 (Олександр) — позначити заняття проведеним. FR-06, AC-06.
app.MapPost("/api/consultations/{id:int}/complete", (int id, CompleteConsultationRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Topic))
        return Results.BadRequest(new ApiError("topic_required", "Вкажіть тему консультації."));

    lock (gate)
    {
        var consultation = consultations.FirstOrDefault(c => c.Id == id);
        if (consultation is null)
            return Results.NotFound(new ApiError("consultation_not_found", "Запис не знайдено."));

        if (consultation.Status != ConsultationStatus.Scheduled)
            return Results.BadRequest(new ApiError("already_completed", "Запис уже завершено або скасовано."));

        consultation.Status = ConsultationStatus.Completed;
        consultation.Topic = request.Topic.Trim();
        return Results.Ok(ToConsultationDto(consultation));
    }
});

app.Run();

static StudentDto ToStudentDto(Student s) => new(s.Id, s.FullName, s.Phone, s.Notes);
static ConsultationDto ToConsultationDto(Consultation c) =>
    new(c.Id, c.StudentId, c.StartDateTime, c.DurationMinutes, c.Status, c.Topic, c.Notes);

// Потрібно для WebApplicationFactory<Program> у тестовому проєкті.
public partial class Program { }
