namespace ScooterRental.Persistence.Data.Configurations
{
    internal class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.FullName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.NationalIdHash)
                .HasMaxLength(255);

            builder.Property(u => u.IdPhotoUrl)
                .HasMaxLength(500);

            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(u => u.IdVerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(u => u.AccountStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();
        }
    }
}
