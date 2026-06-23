namespace ScooterRental.Service
{
    public class ZoneCacheService(IServiceScopeFactory _scopeFactory) : IZoneCacheService
    {
        private IReadOnlyList<ZoneCacheItem> _cachedZones = new List<ZoneCacheItem>();

        public async Task ReloadCacheAsync()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var spec = new AllZonesSpecification(true);

                var zones = await unitOfWork.GetRepository<Zone>().GetAllWithSpecAsync(spec);

                var cacheItems = new List<ZoneCacheItem>(zones.Count);

                foreach (var zone in zones)
                {
                    var cacheItem = new ZoneCacheItem(zone.Id,zone.Name, zone.Type, zone.SpeedLimitKmH, zone.Boundary);

                    cacheItems.Add(cacheItem);
                }
                _cachedZones = cacheItems;
            }
        }

        public IReadOnlyList<ZoneDto> GetZonesForPoint(double longitude, double latitude)
        {
            var targetPoint = new Point(longitude, latitude) { SRID = 4326 };

            var zones = _cachedZones.Where(z => z.Boundary.Contains(targetPoint)).ToList();

            var dtos = zones.ToDtoList();

            return dtos;
        }
    }
}
