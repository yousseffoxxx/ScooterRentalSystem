namespace ScooterRental.Shared.DTOs.Ride.Request
{
    public record EndRideRequestDto(double UserLatitude, double UserLongitude, string EndPhotoUrl)
    {
    }
}
