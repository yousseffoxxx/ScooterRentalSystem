namespace ScooterRental.Presentation.Hubs
{
    [Authorize]
    public class RiderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{Context.UserIdentifier}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{Context.UserIdentifier}");

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRideGroup(string rideId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"Ride_{rideId}");

        public async Task LeaveRideGroup(string rideId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Ride_{rideId}");
    }
}
