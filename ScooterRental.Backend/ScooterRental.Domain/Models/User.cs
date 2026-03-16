namespace ScooterRental.Domain.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        
        public string? NationalIdHash { get; set; }
        
        public string? IdPhotoUrl { get; set; }
        public string? AvatarUrl { get; set; }
        
        public IdVerificationStatus IdVerificationStatus { get; set; } = IdVerificationStatus.Pending;
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
                
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        
        public Wallet Wallet { get; set; } = null!;
    }
}
