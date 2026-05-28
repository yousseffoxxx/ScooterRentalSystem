namespace ScooterRental.Shared.DTOs.Payment.Request
{
    public record Order(int Id, Guid MerchantOrderId)
    {
    }
}
