namespace ScooterRental.Shared.DTOs.Ride.Response
{
    public record PendingParkingPhotoDto(Guid RideId, string EndPhotoUrl, string ScooterSerialNumber, string UserPhoneNumber,DateTimeOffset EndTime)
    {
    }
}
