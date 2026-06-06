namespace ScooterRental.Service
{
    public class ScooterTelemetryService(IScooterTelemetryRepository _repository, IZoneCacheService _zoneCacheService,
        IMqttCommandService _mqttCommandService, ILogger<ScooterTelemetryService> _logger, INotificationService _notificationService,
        IUnitOfWork _unitOfWork,IActiveRideCacheRepository _activeRideCacheRepository) 
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

            if (telemetry.Alarm)
            {
                _logger.LogWarning("ALARM TRIGGERED for Scooter {Serial}", telemetry.SerialNumber);

                var alert = new ScooterAlert
                {
                    SerialNumber = telemetry.SerialNumber,
                    AlertType = "Theft Alarm Triggered"
                };
                _unitOfWork.GetRepository<ScooterAlert>().Add(alert);

                await _unitOfWork.SaveChangesAsync();

                // TODO Future enhancement: Send a WebSocket push directly to the Admin Dashboard!
            }
            await HandleGeofencing(telemetry);

            await _repository.SaveOrUpdateTelemetryAsync(telemetry);
        }

        private async Task HandleGeofencing(ScooterTelemetry telemetry)
        {
            var zones = _zoneCacheService.GetZonesForPoint(telemetry.Longitude, telemetry.Latitude);

            var previousState = await _repository.GetLatestTelemetryAsync(telemetry.SerialNumber);

            var activeRide = await _activeRideCacheRepository.GetActiveRideAsync(telemetry.SerialNumber);
            
            if (activeRide is null)
                return;

            // scooter have left the operational zone
            if (!zones.Any())
            {
                _logger.LogWarning("VIOLATION: Scooter {SerialNumber} entered OUT OF BOUNDS area:", telemetry.SerialNumber);

                if (!string.IsNullOrEmpty(activeRide.FcmToken))
                    await _notificationService.SendNotificationAsync(activeRide.FcmToken, "Scooter OUT OF Bounds",
                    "Warning! You have left the operational zone. The scooter will safely power down.");

                if (previousState == null || previousState.IsOutOfBounds == false)
                    await _mqttCommandService.SendCommandAsync(telemetry.SerialNumber, ScooterCommandType.StopScooter, 0);
                
                telemetry.IsOutOfBounds = true;
            }
            else
            {
                // Check if scooter returned to the operational area
                if (previousState != null && previousState.IsOutOfBounds == true)
                {
                    await _mqttCommandService.SendCommandAsync(telemetry.SerialNumber, ScooterCommandType.StartScooter);

                    if (!string.IsNullOrEmpty(activeRide.FcmToken))
                        await _notificationService.SendNotificationAsync(activeRide.FcmToken, "Back in zone", "You returned to the operational area.");
                    
                    telemetry.IsOutOfBounds = false;
                    
                    _logger.LogInformation("Scooter {Id} returned to operational area. Unlocking.", telemetry.SerialNumber);
                }

                // Evaluate the No-Parking (Red Zone) state
                if (zones.Any(z => z.Type == ZoneType.NoParking.ToString()))
                {
                    // Did we JUST enter it?
                    if (previousState == null || previousState.IsInNoParkingZone == false)
                    {
                        if (!string.IsNullOrEmpty(activeRide.FcmToken))
                            await _notificationService.SendNotificationAsync(activeRide.FcmToken, "NO PARKING ZONE", "Warning! No Parking Zone. You cannot end your ride in this area.");
                        
                        telemetry.IsInNoParkingZone = true;
                    }
                }
                // We are in the Green zone, NOT the Red Zone
                else
                {
                    // Did we JUST leave the Red Zone?
                    if (previousState != null && previousState.IsInNoParkingZone == true)
                    {
                        if (!string.IsNullOrEmpty(activeRide.FcmToken))
                            await _notificationService.SendNotificationAsync(activeRide.FcmToken, "Left No Parking", "You have left the No Parking zone.");
                        
                        telemetry.IsInNoParkingZone = false;
                        
                        _logger.LogInformation("Scooter {Id} have left the No Parking zone", telemetry.SerialNumber);
                    }
                }
            }
        }
    }
}
