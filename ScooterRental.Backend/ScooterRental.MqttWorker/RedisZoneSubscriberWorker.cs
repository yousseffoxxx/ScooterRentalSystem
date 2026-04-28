namespace ScooterRental.MqttWorker
{
    public class RedisZoneSubscriberWorker(IConnectionMultiplexer _connection, IZoneCacheService _zoneCacheService, 
        ILogger<RedisZoneSubscriberWorker> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _zoneCacheService.ReloadCacheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reload zone cache from Redis event.");
            }

            var subscriber = _connection.GetSubscriber();

            await subscriber.SubscribeAsync(RedisChannel.Literal("zone-updates-channel"), async (channel, message) =>
            {
                try
                {
                    _logger.LogInformation("Zone update received via Redis.Reloading map cache");

                    await _zoneCacheService.ReloadCacheAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reload zone cache from Redis event.");          
                }
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
