namespace ScooterRental.Persistence.Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Scooter> Scooters { get; set; }
        public DbSet<ScooterModel> ScooterModels { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
    }
}
