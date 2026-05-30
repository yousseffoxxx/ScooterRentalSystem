namespace ScooterRental.Service.Abstractions.PaymentServices
{
    public interface IPaymobService
    {
        Task<TopUpResponseDto> InitiateWalletPaymentAsync(decimal amount, string phoneNumber, string userId);
        Task<bool> ProcessPaymobWebhook(string hmacFromRequest, string jsonBody);
        Task<bool> AdjustWalletBalanceAsync(AdminWalletAdjustmentDto dto);
        Task<PaginatedResult<WalletTransactionDto>> GetUserTransactionsAsync(Guid userId, QueryParams queryParams);
    }
}
