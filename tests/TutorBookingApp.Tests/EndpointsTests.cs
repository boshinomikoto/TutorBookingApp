using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TutorBookingApp.Api.Contracts;
using TutorBookingApp.Api.Models;
using Xunit;

namespace TutorBookingApp.Tests
{
    // Кожен тест піднімає окремий примірник сервера (WebApplicationFactory),
    // тому робочі дані (лише в пам'яті, як і в самому сервері) не переходять
    // з одного тесту в інший.

   //це просто готовий набір налаштувань JSON для тестів, який повторює те, як сервер серіалізує дані
   //(camelCase + enum як текст),
   //щоб тести могли правильно прочитати відповідь сервера.
    internal static class TestJson
    {
        public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };
    }

    /// <summary>UC-01 (Максим) — реєстрація учня. FR-01, AC-01.</summary>
    public class RegisterStudentTests
    {
        [Fact]
        public async Task ReturnsCreated_WhenFullNameProvided()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/students",
                new RegisterStudentRequest("Іван Петренко", "0671234567", null));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var student = await response.Content.ReadFromJsonAsync<StudentDto>();
            Assert.NotNull(student);
            Assert.Equal("Іван Петренко", student!.FullName);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenFullNameMissing()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/students",
                new RegisterStudentRequest("   ", null, null));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            Assert.Equal("full_name_required", error!.Code);
        }
    }

    /// <summary>UC-03 (Вадим) — створення запису і перевірка перетину часу. FR-03, FR-04, AC-03, AC-04.</summary>
    public class CreateConsultationTests
    {
        [Fact]
        public async Task ReturnsCreated_WithScheduledStatus_WhenNoOverlap()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            var studentId = await RegisterStudentAsync(client, "Олена Коваль");

            var response = await client.PostAsJsonAsync("/api/consultations",
                new CreateConsultationRequest(studentId, new DateTime(2026, 11, 12, 9, 0, 0), 45, null, null));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var consultation = await response.Content.ReadFromJsonAsync<ConsultationDto>(TestJson.Options);
            Assert.Equal(ConsultationStatus.Scheduled, consultation!.Status);
        }

        [Fact]
        public async Task ReturnsConflict_WhenTimeOverlapsAnotherScheduledRecord()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            var studentId = await RegisterStudentAsync(client, "Олена Коваль");

            await client.PostAsJsonAsync("/api/consultations",
                new CreateConsultationRequest(studentId, new DateTime(2026, 11, 12, 9, 0, 0), 60, null, null));

            // Перетинається з попереднім записом (10:00 у межах 09:00-10:00).
            var response = await client.PostAsJsonAsync("/api/consultations",
                new CreateConsultationRequest(studentId, new DateTime(2026, 11, 12, 9, 30, 0), 30, null, null));

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            Assert.Equal("schedule_conflict", error!.Code);

            // Стан даних до/після відмови: другий запис не зберігся.
            var all = await client.GetFromJsonAsync<ConsultationDto[]>("/api/consultations", TestJson.Options);
            Assert.Single(all!);
        }

        private static async Task<int> RegisterStudentAsync(HttpClient client, string fullName)
        {
            var response = await client.PostAsJsonAsync("/api/students", new RegisterStudentRequest(fullName, null, null));
            var student = await response.Content.ReadFromJsonAsync<StudentDto>();
            return student!.Id;
        }
    }

    /// <summary>UC-05 (Олександр) — позначення заняття проведеним. FR-06, AC-06.</summary>
    public class CompleteConsultationTests
    {
        [Fact]
        public async Task ReturnsOk_AndSetsCompleted_WhenTopicProvided()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            var consultationId = await CreateScheduledConsultationAsync(client);

            var response = await client.PostAsJsonAsync(
                $"/api/consultations/{consultationId}/complete",
                new CompleteConsultationRequest("Похідні та інтеграли"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var consultation = await response.Content.ReadFromJsonAsync<ConsultationDto>(TestJson.Options);
            Assert.Equal(ConsultationStatus.Completed, consultation!.Status);
            Assert.Equal("Похідні та інтеграли", consultation.Topic);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenTopicMissing()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            var consultationId = await CreateScheduledConsultationAsync(client);

            var response = await client.PostAsJsonAsync(
                $"/api/consultations/{consultationId}/complete",
                new CompleteConsultationRequest(""));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            Assert.Equal("topic_required", error!.Code);

            // Стан не змінився — запис лишився Scheduled.
            var all = await client.GetFromJsonAsync<ConsultationDto[]>("/api/consultations", TestJson.Options);
            Assert.Equal(ConsultationStatus.Scheduled, all![0].Status);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenAlreadyCompleted()
        {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            var consultationId = await CreateScheduledConsultationAsync(client);
            await client.PostAsJsonAsync($"/api/consultations/{consultationId}/complete",
                new CompleteConsultationRequest("Перша тема"));

            var response = await client.PostAsJsonAsync($"/api/consultations/{consultationId}/complete",
                new CompleteConsultationRequest("Друга тема"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            Assert.Equal("already_completed", error!.Code);
        }

        private static async Task<int> CreateScheduledConsultationAsync(HttpClient client)
        {
            var studentResponse = await client.PostAsJsonAsync("/api/students",
                new RegisterStudentRequest("Тестовий учень", null, null));
            var student = await studentResponse.Content.ReadFromJsonAsync<StudentDto>();

            var consultationResponse = await client.PostAsJsonAsync("/api/consultations",
                new CreateConsultationRequest(student!.Id, new DateTime(2026, 11, 12, 9, 0, 0), 45, null, null));
            var consultation = await consultationResponse.Content.ReadFromJsonAsync<ConsultationDto>(TestJson.Options);
            return consultation!.Id;
        }
    }
}