namespace ScooterRental.Service.Mappings
{
    public static class ZoneMappingExtensions
    {
        private static readonly GeometryFactory _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        private static Polygon CreatePolygon(IEnumerable<CoordinateDto> boundary)
        {
            var coordinates = boundary
                .Select(c => new Coordinate(c.Longitude, c.Latitude))
                .ToList();

            var firstPoint = coordinates.First();
            var lastPoint = coordinates.Last();

            if (!firstPoint.Equals2D(lastPoint))
            {
                coordinates.Add(new Coordinate(firstPoint.X, firstPoint.Y));
            }

            var shell = _geometryFactory.CreateLinearRing(coordinates.ToArray());

            return _geometryFactory.CreatePolygon(shell);
        }
        private static IEnumerable<CoordinateDto> ExtractCoordinates(Polygon polygon)
        {
            if (polygon == null || polygon.ExteriorRing == null)
            {
                return Enumerable.Empty<CoordinateDto>();
            }
            return polygon.ExteriorRing.Coordinates.Select(c => new CoordinateDto(c.X, c.Y));
        }

        // 1. Zone -> ZoneDto
        public static ZoneDto ToDto(this Zone zone)
        {
            return new ZoneDto(
                    zone.Id,
                    zone.Name,
                    zone.Type.ToString(),
                    zone.SpeedLimitKmH,
                    zone.IsActive,
                    ExtractCoordinates(zone.Boundary)
                );
        }

        // 2. ZoneForCreationDto -> Zone
        public static Zone ToEntity(this ZoneForCreationDto dto)
        {
            return new Zone
            {
                Name = dto.Name,
                Type = Enum.Parse<ZoneType>(dto.Type, true),
                SpeedLimitKmH = dto.SpeedLimitKmH,
                Boundary = CreatePolygon(dto.Boundary)
            };
        }

        // 3. ZoneForUpdateDto -> Zone
        public static void UpdateEntity(this ZoneForUpdateDto dto, Zone zone)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name))
                zone.Name = dto.Name;
            
            if (!string.IsNullOrWhiteSpace(dto.Type))
                zone.Type = Enum.Parse<ZoneType>(dto.Type, true);

            if (dto.SpeedLimitKmH is not null)
                zone.SpeedLimitKmH = dto.SpeedLimitKmH;

            if (dto.Boundary != null && dto.Boundary.Any())
                zone.Boundary = CreatePolygon(dto.Boundary);

            zone.IsActive = dto.IsActive;
        }

        // 4. List of Zone -> List of ZoneDto
        public static IReadOnlyList<ZoneDto> ToDtoList(this IReadOnlyList<Zone> zones)
        {
            if (zones == null || zones.Count == 0)
                return new List<ZoneDto>(0);

            var zoneDtos = new List<ZoneDto>(zones.Count);

            foreach (var zone in zones)
            {
                zoneDtos.Add(new ZoneDto(

                    zone.Id,
                    zone.Name,
                    zone.Type.ToString(),
                    zone.SpeedLimitKmH,
                    zone.IsActive,
            ExtractCoordinates(zone.Boundary)
                ));
            }
            return zoneDtos;
        }

        // 5. ZoneCacheItem -> ZoneDto
        public static ZoneDto ToDto(this ZoneCacheItem zoneCacheItem)
        {
            return new ZoneDto(
                    zoneCacheItem.Id,
                    zoneCacheItem.Name,
                    zoneCacheItem.Type.ToString(),
                    zoneCacheItem.SpeedLimitKmH,
                    true,
                    ExtractCoordinates(zoneCacheItem.Boundary)
                );
        }

        // 6. List of ZoneCacheItem -> List of ZoneDto
        public static IReadOnlyList<ZoneDto> ToDtoList(this IReadOnlyList<ZoneCacheItem> zones)
        {
            if (zones == null || zones.Count == 0)
                return new List<ZoneDto>(0);

            var zoneDtos = new List<ZoneDto>(zones.Count);

            foreach (var zone in zones)
            {
                zoneDtos.Add(new ZoneDto(

                    zone.Id,
                    zone.Name,
                    zone.Type.ToString(),
                    zone.SpeedLimitKmH,
                    true,
            ExtractCoordinates(zone.Boundary)
                ));
            }
            return zoneDtos;
        }

    }
}
