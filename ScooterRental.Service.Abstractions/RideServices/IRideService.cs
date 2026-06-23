namespace ScooterRental.Service.Abstractions.RideServices
{
    public interface IRideService
    {
        Task<ActiveRideResponseDto> StartRideAsync(StartRideRequestDto request, Guid userId);
        Task<ActiveRideResponseDto> GetCurrentActiveRideAsync(Guid userId);
        Task<RideDto> EndRideAsync(EndRideRequestDto request, Guid userId);
        Task<PaginatedResult<PendingParkingPhotoDto>> GetPendingParkingPhotosAsync(QueryParams queryParams);
        Task<PaginatedResult<RideDto>> GetAllRidesAsync(QueryParams queryParams);
        Task ReviewParkingPhotoAsync(Guid rideId, ReviewParkingPhotoDto dto);
    }
}
