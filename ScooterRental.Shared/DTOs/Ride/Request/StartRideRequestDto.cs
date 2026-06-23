namespace ScooterRental.Shared.DTOs.Ride.Request
{
    public record StartRideRequestDto(string SerialNumber, double UserLatitude, double UserLongitude)
    {
    }
}
