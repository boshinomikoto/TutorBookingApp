using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

var baseUrl = Environment.GetEnvironmentVariable("TUTORBOOKING_API_URL") ?? "http://localhost:5080";
using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
var json = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new JsonStringEnumConverter() },
};

Console.WriteLine($"TutorBookingApp — консольний клієнт. Сервер: {baseUrl}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Зареєструвати учня (UC-01)");
    Console.WriteLine("2 - Створити запис на консультацію (UC-03)");
    Console.WriteLine("3 - Позначити запис проведеним (UC-05)");
    Console.WriteLine("4 - Список учнів");
    Console.WriteLine("5 - Список записів");
    Console.WriteLine("0 - Вийти");
    Console.Write("> ");

    var choice = Console.ReadLine();
    try
    {
        switch (choice)
        {
            case "1": await RegisterStudentAsync(); break;
            case "2": await CreateConsultationAsync(); break;
            case "3": await CompleteConsultationAsync(); break;
            case "4": await ListStudentsAsync(); break;
            case "5": await ListConsultationsAsync(); break;
            case "0": return;
            default: Console.WriteLine("Не зрозумів, спробуй ще раз."); break;
        }
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Сервер недоступний: {ex.Message}");
    }
}

async Task RegisterStudentAsync()
{
    Console.Write("Повне ім'я: ");
    var fullName = Console.ReadLine();
    Console.Write("Телефон (можна пропустити): ");
    var phone = Console.ReadLine();

    var response = await http.PostAsJsonAsync("/api/students", new { fullName, phone }, json);
    await PrintResultAsync(response);
}

async Task CreateConsultationAsync()
{
    Console.Write("Id учня: ");
    if (!int.TryParse(Console.ReadLine(), out var studentId))
    {
        Console.WriteLine("Некоректний Id.");
        return;
    }

    Console.Write("Початок (yyyy-MM-dd HH:mm): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var start))
    {
        Console.WriteLine("Некоректна дата/час.");
        return;
    }

    Console.Write("Тривалість, хв: ");
    if (!int.TryParse(Console.ReadLine(), out var duration))
    {
        Console.WriteLine("Некоректна тривалість.");
        return;
    }

    var response = await http.PostAsJsonAsync(
        "/api/consultations",
        new { studentId, startDateTime = start, durationMinutes = duration, topic = (string?)null, notes = (string?)null },
        json);
    await PrintResultAsync(response);
}

async Task CompleteConsultationAsync()
{
    Console.Write("Id запису: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Некоректний Id.");
        return;
    }

    Console.Write("Тема консультації: ");
    var topic = Console.ReadLine();

    var response = await http.PostAsJsonAsync($"/api/consultations/{id}/complete", new { topic }, json);
    await PrintResultAsync(response);
}

async Task ListStudentsAsync()
{
    var response = await http.GetAsync("/api/students");
    await PrintResultAsync(response);
}

async Task ListConsultationsAsync()
{
    var response = await http.GetAsync("/api/consultations");
    await PrintResultAsync(response);
}

static async Task PrintResultAsync(HttpResponseMessage response)
{
    var body = await response.Content.ReadAsStringAsync();
    if (response.IsSuccessStatusCode)
        Console.WriteLine($"OK ({(int)response.StatusCode}): {body}");
    else
        Console.WriteLine($"Відмова ({(int)response.StatusCode}): {body}");
}
