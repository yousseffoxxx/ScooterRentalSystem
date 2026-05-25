namespace ScooterRental.Shared.DTOs.Payment
{
    public record TopUpRequestDto(decimal Amount, string WalletPhoneNumber)
    {
    }
}
