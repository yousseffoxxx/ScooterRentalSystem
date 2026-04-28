namespace ScooterRental.Persistence.Data.Configurations
{
    public class ScooterConfigurations : IEntityTypeConfiguration<Scooter>
    {
        public void Configure(EntityTypeBuilder<Scooter> builder)
        {

            builder.ToTable("Scooters");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SerialNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.CurrentBatteryLevel)
                .IsRequired();

            builder.Property(s=>s.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(s => s.Location)
                .HasColumnType("geography");

            builder.Property(s => s.LastPingAt)
                .IsRequired();

            builder.HasIndex(s => s.SerialNumber)
                .IsUnique();

            builder.HasOne(s => s.Model)
                .WithMany(sm => sm.Scooters)
                .HasForeignKey(s => s.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
