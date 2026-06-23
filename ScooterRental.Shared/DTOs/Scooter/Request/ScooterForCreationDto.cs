namespace ScooterRental.Shared.DTOs.Scooter.Request
{
    public record ScooterForCreationDto(string SerialNumber, Guid ModelId)
    {
    }
}
