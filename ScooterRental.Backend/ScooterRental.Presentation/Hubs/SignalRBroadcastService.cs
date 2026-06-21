namespace ScooterRental.Presentation.Hubs
{
    public class SignalRBroadcastService(IHubContext<AdminHub> _adminHub, IHubContext<RiderHub> _riderHub) : IRealTimeBroadcastService
    {
        public async Task BroadcastLiveTelemetryToAdminsAsync(MapScooterDto scooter)
            => await _adminHub.Clients.Group("Admins").SendAsync("ReceiveLiveTelemetry", scooter);

        public async Task BroadcastRideTelemetryToRiderAsync(string rideId, MapScooterDto scooter)
            => await _riderHub.Clients.Group($"Ride_{rideId}").SendAsync("ReceiveRideTelemetry", scooter);

        public async Task BroadcastSecurityAlertToAdminsAsync(string serialNumber, string alertMessage)
        
           => await _adminHub.Clients.Group("Admins").SendAsync("ReceiveSecurityAlert", new{SerialNumber = serialNumber, Message = alertMessage, Timestamp = DateTimeOffset.UtcNow });
        
        public async Task BroadcastWalletTopUpToRiderAsync(string userId, decimal newBalance)
            => await _riderHub.Clients.Group($"User_{userId}").SendAsync("WalletBalanceUpdated", newBalance);
    }
}
