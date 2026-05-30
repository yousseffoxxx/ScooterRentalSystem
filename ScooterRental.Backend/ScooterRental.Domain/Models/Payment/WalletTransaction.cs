namespace ScooterRental.Domain.Models.Payment
{
    public class WalletTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? ReferenceId { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;
    }
}
