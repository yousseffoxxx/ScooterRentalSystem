namespace ScooterRental.Service.Abstractions
{
    public interface IRedisZoneEventPublisher
    {
        Task PublishZoneCacheInvalidationAsync(ZoneCacheInvalidatedEvent eventMessage);
    }
}
