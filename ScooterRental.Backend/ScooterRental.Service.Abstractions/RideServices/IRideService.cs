namespace ScooterRental.Service.Abstractions.RideServices
{
    public interface IRideService
    {
        Task<ActiveRideResponseDto> StartRideAsync(StartRideRequestDto request, Guid userId);
        Task<ActiveRideResponseDto> GetCurrentActiveRideAsync(Guid userId);
        Task<RideDto> EndRideAsync(EndRideRequestDto request, Guid userId);
    }
}
