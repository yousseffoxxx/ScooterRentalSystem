namespace ScooterRental.Domain.Models.Scooters
{
    public record ZoneCacheItem(Guid Id,string Name, ZoneType Type, double? SpeedLimitKmH, Polygon Boundary)
    {
    }
}
