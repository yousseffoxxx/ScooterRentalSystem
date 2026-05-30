namespace ScooterRental.Shared.DTOs.Tariff.Request
{
    public record TariffForCreationDto(string Name, decimal UnlockFee, decimal PerMinuteRate)
    {
    }
}
