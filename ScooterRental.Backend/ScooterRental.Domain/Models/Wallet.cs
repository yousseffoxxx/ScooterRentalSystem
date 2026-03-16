namespace ScooterRental.Domain.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; } = 0.00M;
        public decimal HeldAmount { get; set; } = 0.00M;
        public decimal TotalSpent { get; set; } = 0.00M;
        public decimal TotalToppedUp { get; set; } = 0.00M;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
