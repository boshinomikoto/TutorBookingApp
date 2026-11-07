namespace TutorBookingApp.Api.Contracts
{
    /// <summary>Повідомлення про відмову: код причини та текст для користувача.</summary>
    public sealed record ApiError(string Code, string Message);
}
