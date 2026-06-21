namespace ScooterRental.Service.Abstractions.RealTimeServices
{
    public interface IRealTimeBroadcastService
    {
        Task BroadcastLiveTelemetryToAdminsAsync(MapScooterDto scooter);
        Task BroadcastRideTelemetryToRiderAsync(string rideId, MapScooterDto scooter);
        Task BroadcastSecurityAlertToAdminsAsync(string serialNumber, string alertMessage);
        Task BroadcastWalletTopUpToRiderAsync(string userId, decimal newBalance);
    }
}
