namespace ScooterRental.Shared.DTOs.Tariff.Response
{
    public record TariffDto(Guid Id, string Name, decimal UnlockFee, decimal PerMinuteRate, bool IsActive, DateTimeOffset CreatedAt)
    {
    }
}
