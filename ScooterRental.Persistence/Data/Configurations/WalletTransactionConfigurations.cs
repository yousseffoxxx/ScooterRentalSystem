namespace ScooterRental.Persistence.Data.Configurations
{
    public class WalletTransactionConfigurations : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.Type)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.ReferenceId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(t => t.Timestamp)
                .IsRequired();

            builder.HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => t.ReferenceId)
                .IsUnique(false);
        }
    }
}
