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

            builder.Services.AddHostedService<MqttTelemetryWorker>();
            builder.Services.AddHostedService<RedisZoneSubscriberWorker>();

            builder.Services.AddScoped<IMqttCommandService, MqttCommandService>();
            builder.Services.AddScoped<IScooterTelemetryRepository, ScooterTelemetryRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IScooterTelemetryService, ScooterTelemetryService>();
            builder.Services.AddSingleton<IZoneCacheService, ZoneCacheService>();
            var host = builder.Build();

            host.Run();
        }
    }
}