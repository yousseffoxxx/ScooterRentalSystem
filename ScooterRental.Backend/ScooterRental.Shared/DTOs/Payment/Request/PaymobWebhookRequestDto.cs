namespace ScooterRental.Shared.DTOs.Payment.Request
{
    public record PaymobWebhookRequestDto(string Type, PaymobTransactionObjDto ObjDto)
    {
    }
}
