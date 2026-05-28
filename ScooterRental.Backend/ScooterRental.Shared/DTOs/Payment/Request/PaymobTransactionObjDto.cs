namespace ScooterRental.Shared.DTOs.Payment.Request
{
    public record PaymobTransactionObjDto(int Id, int AmountCents, bool Success, bool Pending, Order Order)
    {
    }
}
