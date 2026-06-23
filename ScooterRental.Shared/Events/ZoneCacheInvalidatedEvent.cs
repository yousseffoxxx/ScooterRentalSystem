namespace ScooterRental.Shared.Events
{
    public record ZoneCacheInvalidatedEvent(Guid ZoneId, string Action)
    {
    }
}
