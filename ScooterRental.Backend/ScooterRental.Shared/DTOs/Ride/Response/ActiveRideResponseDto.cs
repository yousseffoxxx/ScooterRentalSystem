namespace ScooterRental.Shared.DTOs.Ride.Response
{
    public record ActiveRideResponseDto(Guid RideId, string ScooterSerialNumber, DateTimeOffset StartTime, decimal CurrentDurationMinutes, decimal CurrentCost)
    {
    }
}
