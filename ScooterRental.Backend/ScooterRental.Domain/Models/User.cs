namespace ScooterRental.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool PhoneVerified { get; set; } = false;
        public string? NationalIdHash { get; set; }
        public IdVerificationStatus IdVerificationStatus { get; set; } = IdVerificationStatus.Pending;
        public string IdPhotoUrl { get; set; } = null!;
        public string? AvatarUrl { get; set; } = null!;
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public int FailedLoginCount { get; set; } = 0;
        public DateTimeOffset? LockoutUntil { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;
        public Wallet Wallet { get; set; } = null!;
        public Guid WalletId { get; set; }
    }
}
