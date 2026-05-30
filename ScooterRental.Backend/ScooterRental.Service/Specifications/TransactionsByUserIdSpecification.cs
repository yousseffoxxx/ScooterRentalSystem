namespace ScooterRental.Service.Specifications
{
    public class TransactionsByUserIdSpecification : BaseSpecifications<WalletTransaction>
    {
        public TransactionsByUserIdSpecification(Guid userId, int pageIndex, int pageSize) : base(t => t.Wallet.UserId == userId)
        {
            AddOrderByDescending(t => t.Timestamp);

            ApplyPagination(pageIndex, pageSize);
        }

        public TransactionsByUserIdSpecification(Guid userId) : base(t => t.Wallet.UserId == userId)
        {
        }
    }
}
