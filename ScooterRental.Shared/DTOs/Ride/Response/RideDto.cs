namespace ScooterRental.Shared.DTOs.Ride.Response
{
    public record RideDto(Guid Id, string ScooterSerialNumber, string UserPhoneNumber, DateTimeOffset StartTime, DateTimeOffset? EndTime, decimal? DurationInMinutes, decimal? TotalCost, string Status, string? EndPhotoUrl)
    {
    }
}
