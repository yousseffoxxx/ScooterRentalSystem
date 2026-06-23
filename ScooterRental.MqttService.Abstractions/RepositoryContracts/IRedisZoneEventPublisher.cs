namespace ScooterRental.MqttService.Abstractions
{
    public interface IRedisZoneEventPublisher
    {
        Task PublishZoneCacheInvalidationAsync(ZoneCacheInvalidatedEvent eventMessage);
    }
}
