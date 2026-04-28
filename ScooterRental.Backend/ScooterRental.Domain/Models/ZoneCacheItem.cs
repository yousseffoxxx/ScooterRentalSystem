namespace ScooterRental.Domain.Models
{
    public record ZoneCacheItem(Guid Id,string Name, ZoneType Type, double? SpeedLimitKmH, Polygon Boundary)
    {
    }
}
