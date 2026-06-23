namespace ScooterRental.Shared.DTOs.AdminManagement.Payment.Response
{
    public record WalletTransactionDto(Guid Id, decimal Amount, string Type, string? ReferenceId, string? Description, DateTimeOffset Timestamp)
    {
    }
}
