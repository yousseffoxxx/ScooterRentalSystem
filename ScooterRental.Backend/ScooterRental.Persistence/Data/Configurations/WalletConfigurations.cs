using ScooterRental.Domain.Models.Payment;

namespace ScooterRental.Persistence.Data.Configurations
{
    internal class WalletConfigurations : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");

            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(w => w.Balance)
                .HasColumnType("decimal(12,2)")
                .HasDefaultValue(0.00M);

            builder.Property(w => w.HeldAmount)
                .HasColumnType("decimal(12,2)")
                .HasDefaultValue(0.00M);

            builder.Property(w => w.TotalSpent)
                .HasColumnType("decimal(12,2)")
                .HasDefaultValue(0.00M);

            builder.Property(w => w.TotalToppedUp)
                .HasColumnType("decimal(12,2)")
                .HasDefaultValue(0.00M);

            builder.Property(w => w.UpdatedAt)
                .IsRequired();
        }
    }
}
