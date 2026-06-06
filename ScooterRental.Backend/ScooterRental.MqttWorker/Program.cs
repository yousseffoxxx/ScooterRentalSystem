using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace ScooterRental.MqttWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("MqttSettings"));

            builder.Services.AddSerilog((config) =>
            {
                config.WriteTo.Console();
                config.WriteTo.Seq("http://localhost:5341");
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    options => options.UseNetTopologySuite());
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>((_) =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnectionString"));
            });

            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
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

            builder.Services.AddHostedService<MqttTelemetryWorker>();
            builder.Services.AddHostedService<RedisZoneSubscriberWorker>();
            builder.Services.AddHostedService<TelemetrySyncWorker>();
            builder.Services.AddHttpClient();

            builder.Services.AddValidatorsFromAssembly(typeof(ZoneForCreationDtoValidator).Assembly);

            // 1. The service that was explicitly crashing the app:
            builder.Services.AddScoped<IAuthService, AuthService>();

            // 2. The other domain services that ServiceManager wraps:
            builder.Services.AddScoped<IActiveRideCacheRepository, ActiveRideCacheRepository>();
            builder.Services.AddScoped<IScooterService, ScooterService>();
            builder.Services.AddScoped<IZoneService, ZoneService>();
            builder.Services.AddScoped<ITariffService, TariffService>();
            builder.Services.AddScoped<IRideService, RideService>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IScooterTelemetryRepository, ScooterTelemetryRepository>();
            builder.Services.AddScoped<IRedisZoneEventPublisher, RedisZoneEventPublisher>();
            builder.Services.AddScoped<IMqttCommandService, MqttCommandService>();
            builder.Services.AddScoped<IDataSeeder, DataSeeder>();
            builder.Services.AddScoped<IPaymobService, PaymobService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IScooterTelemetryService, ScooterTelemetryService>();
            builder.Services.AddSingleton<IZoneCacheService, ZoneCacheService>();

            var firebasePath = builder.Configuration.GetRequiredSection("Firebase")["CredentialPath"];

            if (FirebaseApp.DefaultInstance == null)
            {
                var credential = CredentialFactory.FromFile<ServiceAccountCredential>(firebasePath).ToGoogleCredential();

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }
            var host = builder.Build();


            host.Run();
        }
    }
}