namespace ScooterRental.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.UseNetTopologySuite());
            });

            services.AddSingleton<IConnectionMultiplexer>((_) =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString"));
            });

            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IActiveRideCacheRepository, ActiveRideCacheRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IScooterTelemetryRepository, ScooterTelemetryRepository>();
            services.AddScoped<IRedisZoneEventPublisher, RedisZoneEventPublisher>();
            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddScoped<IScooterSecretCacheRepository, ScooterSecretCacheRepository>();

            return services;
        }
    }
}
