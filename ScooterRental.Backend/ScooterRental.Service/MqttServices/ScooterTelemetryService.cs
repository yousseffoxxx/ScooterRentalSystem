namespace ScooterRental.Service
{
    public class ScooterTelemetryService(IScooterTelemetryRepository _repository, IZoneCacheService _zoneCacheService,
        IMqttCommandService _mqttCommandService, ILogger<ScooterTelemetryService> _logger, INotificationService _notificationService,
        IUnitOfWork _unitOfWork, IActiveRideCacheRepository _activeRideCacheRepository, IScooterSecretCacheRepository _scooterSecretCacheRepository,
        IRealTimeBroadcastService _broadcastService)
        : IScooterTelemetryService
    {
        public async Task ProcessIncomingTelemetryAsync(string jsonPayload)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var telemetry = JsonSerializer.Deserialize<SecureIotPayload<HardwareTelemetryData>>(jsonPayload, options);

            if (telemetry == null)
            {
                _logger.LogWarning("Received empty or invalid telemetry payload. Skipping.");
                return;
            }

            if (Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - telemetry.Timestamp) > 30)
            {
                _logger.LogWarning("Invalid timestamp detected. Possible replay attack.");
                return;
            }

            var deviceSecretKey = await _scooterSecretCacheRepository.GetSecretAsync(telemetry.SerialNumber);

            Guid scooterId = Guid.Empty;

            if (deviceSecretKey is null)
            {
                var scooterFromDb = await _unitOfWork.GetRepository<Scooter>().GetEntityWithSpecAsync(new ScooterBySerialNumberSpecification(telemetry.SerialNumber));

                if (scooterFromDb is null)
                {
                    _logger.LogWarning("Scooter doesn't exist");
                    return;
                }

                deviceSecretKey = scooterFromDb.DeviceSecretKey;
                scooterId = scooterFromDb.Id;

                await _scooterSecretCacheRepository.SetSecretAsync(scooterFromDb.SerialNumber, scooterFromDb.DeviceSecretKey);
            }

            var keyBytes = Encoding.UTF8.GetBytes(deviceSecretKey);

            var innerJson = JsonSerializer.Serialize(telemetry.Data, options);

            var stringToSign = $"{innerJson}.{telemetry.Timestamp}";

            var payloadBytes = Encoding.UTF8.GetBytes(stringToSign);

            using var hmac = new HMACSHA256(keyBytes);

            var hashBytes = hmac.ComputeHash(payloadBytes);

            var signature = Convert.ToHexString(hashBytes).ToLower();

            if (telemetry.Signature != signature)
            {
                _logger.LogCritical("Wrong Signature");
                return;
            }

            var cacheModel = new ScooterTelemetry
            {
                SerialNumber = telemetry.SerialNumber,
                Timestamp = telemetry.Timestamp,
                BatteryLevel = telemetry.Data.BatteryLevel,
                Latitude = telemetry.Data.Latitude,
                Longitude = telemetry.Data.Longitude,
                Alarm = telemetry.Data.Alarm,
                IsOutOfBounds = false,
                IsInNoParkingZone = false
            };

            if (cacheModel.Alarm)
            {
                _logger.LogWarning("ALARM TRIGGERED for Scooter {Serial}", telemetry.SerialNumber);

                var alert = new ScooterAlert
                {
                    SerialNumber = telemetry.SerialNumber,
                    AlertType = "Theft Alarm Triggered"
                };
                _unitOfWork.GetRepository<ScooterAlert>().Add(alert);

                await _unitOfWork.SaveChangesAsync();

                await _broadcastService.BroadcastSecurityAlertToAdminsAsync(telemetry.SerialNumber, "Theft Alarm Triggered!");
            }

            var mapDto = new MapScooterDto(scooterId, telemetry.SerialNumber, cacheModel.BatteryLevel, cacheModel.Latitude, cacheModel.Longitude, 0, 0);

            await HandleGeofencing(cacheModel, mapDto);

            await _repository.SaveOrUpdateTelemetryAsync(cacheModel);

            await _broadcastService.BroadcastLiveTelemetryToAdminsAsync(mapDto);
        }

        private async Task HandleGeofencing(ScooterTelemetry telemetry, MapScooterDto mapDto)
        {
            var zones = _zoneCacheService.GetZonesForPoint(telemetry.Longitude, telemetry.Latitude);

            var previousState = await _repository.GetLatestTelemetryAsync(telemetry.SerialNumber);

            var activeRide = await _activeRideCacheRepository.GetActiveRideAsync(telemetry.SerialNumber);

            if (activeRide is null)
                return;

            await _broadcastService.BroadcastRideTelemetryToRiderAsync(activeRide.RideId.ToString(), mapDto);

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

                await _broadcastService.BroadcastSecurityAlertToAdminsAsync(telemetry.SerialNumber, "Rider drove OUT OF BOUNDS. Scooter safely disabled.");
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
