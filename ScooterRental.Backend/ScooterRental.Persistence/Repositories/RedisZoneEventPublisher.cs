namespace ScooterRental.Persistence.Repositories
{
    public class RedisZoneEventPublisher(IConnectionMultiplexer _connectionMultiplexer) : IRedisZoneEventPublisher
    {
        public async Task PublishZoneCacheInvalidationAsync(ZoneCacheInvalidatedEvent eventMessage)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();

            var jsonMessage = JsonSerializer.Serialize(eventMessage);

            await subscriber.PublishAsync("zone-updates-channel", jsonMessage);
        }
    }
}
