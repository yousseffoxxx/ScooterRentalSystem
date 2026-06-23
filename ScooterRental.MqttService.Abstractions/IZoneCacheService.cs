namespace ScooterRental.MqttService.Abstractions
{
    public interface IZoneCacheService
    {
        Task ReloadCacheAsync();
        IReadOnlyList<ZoneDto> GetZonesForPoint(double longitude, double latitude);
        //void RefreshZones(IEnumerable<Zone> newZones);

        //bool IsInOperationalZone(Point location);

        //Zone? GetNoParkingZoneViolation(Point location);
    }
}
