namespace ScooterRental.Domain.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string? NationalIdHash { get; set; }
        public string? IdFrontPhotoUrl { get; set; }
        public string? IdBackPhotoUrl { get; set; }
        public string? SelfiePhotoUrl { get; set; }
        public string? AvatarUrl { get; set; }
        public string? FcmToken { get; set; }
        public ReviewStatus IdVerificationStatus { get; set; } = ReviewStatus.Pending;
        public string? IdRejectionReason { get; set; }
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        
        public Wallet Wallet { get; set; } = null!;
        public ICollection<Ride> Rides { get; set; } = new List<Ride>();
    }
}
