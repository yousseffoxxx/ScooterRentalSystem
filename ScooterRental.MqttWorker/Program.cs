namespace ScooterRental.MqttWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "appsettings.Shared.json");
            
            builder.Configuration.AddJsonFile(sharedConfigPath, optional: true, reloadOnChange: true);

            builder.Services.AddSerilog((config) =>
            {
                config.WriteTo.Console();

                config.WriteTo.Seq("http://host.docker.internal:5341");
            });

            builder.Services.AddHostedService<MqttTelemetryWorker>();
            builder.Services.AddHostedService<RedisZoneSubscriberWorker>();
            builder.Services.AddHostedService<TelemetrySyncWorker>();
            builder.Services.AddHttpClient();

            builder.Services.AddValidatorsFromAssembly(typeof(ZoneForCreationDtoValidator).Assembly);

            builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("MqttSettings"));

            builder.Services.AddSignalR();

            // 1. Register Data Access (Database, Redis, Repositories)
            builder.Services.AddPersistenceInfrastructure(builder.Configuration);

            // 2. Register Business Logic (Services, Firebase, AI)
            builder.Services.AddApplicationServices(builder.Configuration);

            // 3. Register Presentation (SignalR Implementations, Controllers, etc.)
            builder.Services.AddPresentation();

            var host = builder.Build();

            host.Run();
        }
    }
}