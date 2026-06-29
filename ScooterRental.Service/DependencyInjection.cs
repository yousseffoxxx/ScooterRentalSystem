global using FirebaseAdmin;
global using Google.Apis.Auth.OAuth2;
global using ScooterRental.Service.NotificationServices;
global using ScooterRental.Service.StorageServices;

namespace ScooterRental.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var firebasePath = configuration.GetRequiredSection("Firebase")["CredentialPath"];
            if (FirebaseApp.DefaultInstance == null)
            {
                var credential = CredentialFactory.FromFile<ServiceAccountCredential>(firebasePath).ToGoogleCredential();
                FirebaseApp.Create(new AppOptions { Credential = credential });
            }

            services.AddScoped<IScooterService, ScooterService>();
            services.AddScoped<IZoneService, ZoneService>();
            services.AddScoped<ITariffService, TariffService>();
            services.AddScoped<IRideService, RideService>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddScoped<IMqttCommandService, MqttCommandService>();
            services.AddScoped<IPaymobService, PaymobService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IScooterTelemetryService, ScooterTelemetryService>();
            services.AddScoped<IAiVerificationService, AiVerificationService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddSingleton<IZoneCacheService, ZoneCacheService>();

            return services;
        }
    }
}
