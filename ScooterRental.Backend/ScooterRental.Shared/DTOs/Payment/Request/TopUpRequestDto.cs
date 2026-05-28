namespace ScooterRental.Shared.DTOs.Payment.Request
{
    public record TopUpRequestDto(decimal Amount, string WalletPhoneNumber)
    {
    }
}
