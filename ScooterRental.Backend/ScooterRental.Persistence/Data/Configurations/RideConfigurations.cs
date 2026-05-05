namespace ScooterRental.Persistence.Data.Configurations
{
    public class RideConfigurations : IEntityTypeConfiguration<Ride>
    {
        public void Configure(EntityTypeBuilder<Ride> builder)
        {
            builder.ToTable("Rides");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.StartTime)
                .IsRequired();

            builder.Property(r => r.StartLocation)
                .IsRequired()
                .HasColumnType("geography");

            builder.Property(r => r.EndLocation)
                .HasColumnType("geography");

            builder.Property(r => r.AppliedUnlockFee)
                .IsRequired()
                .HasColumnType("decimal(12,2)");

            builder.Property(r => r.AppliedPerMinuteRate)
                .IsRequired()
                .HasColumnType("decimal(12,2)");

            builder.Property(r => r.TotalCost)
                .HasColumnType("decimal(12,2)");

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.EndPhotoUrl)
                .HasMaxLength(500);

            builder.Property(r => r.DurationMinutes)
                .HasColumnType("decimal(8,2)");

            builder.HasOne(r => r.Scooter)
                .WithMany(s => s.Rides)
                .HasForeignKey(r => r.ScooterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.User)
                .WithMany(u => u.Rides)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
