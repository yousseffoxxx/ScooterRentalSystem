namespace ScooterRental.MqttWorker
{
    public class TelemetrySyncWorker(IServiceProvider _serviceProvider, ILogger<TelemetrySyncWorker> _logger) : BackgroundService
    {
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();

                    var repo = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var telemetryRedisRepo = scope.ServiceProvider.GetRequiredService<IScooterTelemetryRepository>();

                    var scooters = await repo.GetRepository<Scooter>().GetAllAsync();

                    foreach (var scooter in scooters)
                    {
                        var telemetry = await telemetryRedisRepo.GetLatestTelemetryAsync(scooter.SerialNumber);

                        if (telemetry is not null && scooter.LastPingAt < telemetry.Timestamp)
                        {
                            scooter.Location = new Point(telemetry.Longitude, telemetry.Latitude) { SRID = 4326 };
                            scooter.CurrentBatteryLevel = telemetry.BatteryLevel;
                            scooter.LastPingAt = telemetry.Timestamp;
                        }
                    }

                    await repo.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing scooter telemetry from Redis to SQL. Will retry in the next cycle.");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
    }
}
