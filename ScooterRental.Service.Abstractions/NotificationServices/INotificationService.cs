namespace ScooterRental.Service.Abstractions.NotificationServices
{
    public interface INotificationService
    {
        Task<string> SendNotificationAsync(string token, string title, string body, IDictionary<string, string>? hiddenDataPayload = null);
    }
}
