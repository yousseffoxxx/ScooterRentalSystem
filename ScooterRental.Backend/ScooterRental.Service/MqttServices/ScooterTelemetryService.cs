namespace ScooterRental.Service
{
    public class ScooterTelemetryService(IScooterTelemetryRepository _repository, IZoneCacheService _zoneCacheService,
        IMqttCommandService _mqttCommandService, ILogger<ScooterTelemetryService> _logger) 
        : IScooterTelemetryService
    {
        public async Task ProcessIncomingTelemetryAsync(string jsonPayload)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var telemetry = JsonSerializer.Deserialize<ScooterTelemetry>(jsonPayload, options);

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
            var zones = _zoneCacheService.GetZonesForPoint(telemetry.Longitude, telemetry.Latitude);

            var previousState = await _repository.GetLatestTelemetryAsync(telemetry.SerialNumber);

            if (!zones.Any())
            {
                _logger.LogWarning("VIOLATION: Scooter {SerialNumber} entered OUT OF BOUNDS area:", telemetry.SerialNumber);

                if (previousState == null || previousState.IsOutOfBounds == false)
                    await _mqttCommandService.SendCommandAsync(telemetry.SerialNumber, ScooterCommandType.StopScooter, 0);
                
                telemetry.IsOutOfBounds = true;
            }
            else
            {
                _logger.LogInformation("Scooter {SerialNumber} is moving legally within bounds.", telemetry.SerialNumber);

                if (previousState != null && previousState.IsOutOfBounds == true)
                {
                    await _mqttCommandService.SendCommandAsync(telemetry.SerialNumber, ScooterCommandType.StartScooter);

                    _logger.LogInformation("Scooter {Id} returned to operational area. Unlocking.", telemetry.SerialNumber);
                }
            }
        }
    }
}
