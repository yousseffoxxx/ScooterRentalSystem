namespace ScooterRental.Service.Abstractions.PaymentServices
{
    public interface IPaymobService
    {
        Task<TopUpResponseDto> InitiateWalletPaymentAsync(decimal amount, string phoneNumber);
    }
}
