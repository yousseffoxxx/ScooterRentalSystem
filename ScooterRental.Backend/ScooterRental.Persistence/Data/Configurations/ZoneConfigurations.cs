
namespace ScooterRental.Persistence.Data.Configurations
{
    public class ZoneConfigurations : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zones");

            builder.HasKey(z => z.Id);

            builder.Property(z => z.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(z => z.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(z => z.Boundary)
                .IsRequired()
                .HasColumnType("geography");

            builder.Property(z => z.SpeedLimitKmH)
                .HasColumnType("float");

            builder.Property(z => z.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(z => z.CreatedAt)
                .IsRequired();
        }
    }
}
