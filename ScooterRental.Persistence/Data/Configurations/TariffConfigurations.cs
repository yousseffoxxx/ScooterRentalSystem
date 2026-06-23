namespace ScooterRental.Persistence.Data.Configurations
{
    public class TariffConfigurations : IEntityTypeConfiguration<Tariff>
    {
        public void Configure(EntityTypeBuilder<Tariff> builder)
        {
            builder.ToTable("Tariffs");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.UnlockFee)
                .IsRequired()
                .HasColumnType("decimal(12,2)");

            builder.Property(t => t.PerMinuteRate)
                .IsRequired()
                .HasColumnType("decimal(12,2)");

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();
        }
    }
}
