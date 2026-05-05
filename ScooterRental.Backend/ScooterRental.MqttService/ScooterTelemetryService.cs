namespace ScooterRental.MqttService
{
    public class ScooterTelemetryService(IScooterTelemetryRepository _repository, IZoneCacheService _zoneCacheService,
        IMqttCommandService _mqttCommandService, ILogger<ScooterTelemetryService> _logger) 
        : IScooterTelemetryService
    {
        public async Task ProcessIncomingTelemetryAsync(string jsonPayload)
        {
            var telemetry = JsonSerializer.Deserialize<ScooterTelemetry>(jsonPayload);

            if (telemetry == null)
            {
                _logger.LogWarning("Received empty or invalid telemetry payload. Skipping.");
                return;
            }

            await HandleGeofencing(telemetry);

            await _repository.SaveOrUpdateTelemetryAsync(telemetry);
        }

        private async Task HandleGeofencing(ScooterTelemetry telemetry)
        {
            var geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var point = geoFactory.CreatePoint(new Coordinate(telemetry.Longitude, telemetry.Latitude));

            var zone = _zoneCacheService.GetNoParkingZoneViolation(point);

            if (zone is not null)
                _logger.LogWarning("VIOLATION: Scooter {Id} entered NO PARKING zone: {ZoneName}", telemetry.Id, zone.Name);

            else if (!_zoneCacheService.IsInOperationalZone(point))
                await _mqttCommandService.SendCommandAsync(telemetry.SerialNumber, ScooterCommandType.SetSpeedLimit, 5);

            else
                _logger.LogInformation("Scooter {Id} is moving legally within bounds.", telemetry.SerialNumber);
        }
    }
}
