namespace ScooterRental.Domain.Models.Rides
{
    public record ActiveRideCacheModel(Guid RideId, Guid UserId, string SerialNumber, string? FcmToken);
}
