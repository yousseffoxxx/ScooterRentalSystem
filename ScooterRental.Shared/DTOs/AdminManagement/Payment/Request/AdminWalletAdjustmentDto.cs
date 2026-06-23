namespace ScooterRental.Shared.DTOs.AdminManagement.Payment.Request
{
    public record AdminWalletAdjustmentDto(Guid UserId, decimal Amount, string Reason)
    {
    }
}
