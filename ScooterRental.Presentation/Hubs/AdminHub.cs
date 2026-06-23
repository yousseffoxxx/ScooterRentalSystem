namespace ScooterRental.Presentation.Hubs
{
    [Authorize(Roles = "Admin")]
    public class AdminHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");

            await base.OnDisconnectedAsync(exception);
        }
    }
}
