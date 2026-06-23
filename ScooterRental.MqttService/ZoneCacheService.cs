global using ScooterRental.Shared.DTOs.Zone.Response;
global using Microsoft.Extensions.DependencyInjection;
global using ScooterRental.Service.Abstractions.RepositoryContracts;

namespace ScooterRental.MqttService
{
    public class ZoneCacheService(IServiceScopeFactory _scopeFactory) : IZoneCacheService
    {
        private IReadOnlyList<ZoneCacheItem> _cachedZones = new List<ZoneCacheItem>();

        public async Task ReloadCacheAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var zones = await unitOfWork.GetRepository<Zone>().GetAllAsync();
                var cacheItems = new List<ZoneCacheItem>();
                foreach (var zone in zones)
                {
                    if (zone.IsActive)
                        cacheItems.Add(zone);

                }
            }
        }

        public IReadOnlyList<ZoneDto> GetZonesForPoint(double longitude, double latitude)
        {
            throw new NotImplementedException();
        }

        //private List<Zone> _zones = new();
        //private readonly object _lock = new();
        //public void RefreshZones(IEnumerable<Zone> newZones)
        //{
        //    lock (_lock)
        //    {
        //        _zones = newZones.ToList();
        //    }
        //}

        //public bool IsInOperationalZone(Point location)
        //{
        //    lock (_lock)
        //    {
        //        return _zones
        //            .Where(z => z.Type == ZoneType.Operational && z.IsActive)
        //            .Any(z => z.Boundary.Contains(location));
        //    }
        //}

        //public Zone? GetNoParkingZoneViolation(Point location)
        //{
        //    lock (_lock)
        //    {
        //        return _zones
        //            .Where(z => z.Type == ZoneType.NoParking && z.IsActive)
        //            .FirstOrDefault(z => z.Boundary.Contains(location));
        //    }
        //}
    }
}
