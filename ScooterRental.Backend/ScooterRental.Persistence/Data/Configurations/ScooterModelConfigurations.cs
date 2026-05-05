namespace ScooterRental.Persistence.Data.Configurations
{
    public class ScooterModelConfigurations : IEntityTypeConfiguration<ScooterModel>
    {
        public void Configure(EntityTypeBuilder<ScooterModel> builder)
        {
            builder.ToTable("ScooterModels");

            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.Brand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sm => sm.ModelName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sm => sm.MaxSpeedKmH)
                .IsRequired()
                .HasColumnType("decimal(5,2)");
            
            builder.Property(sm => sm.WeightLimitKg)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(sm => sm.BatteryCapacityMah)
                .IsRequired();
        }
    }
}
