namespace ScooterRental.Service.RideServices
{
    public class RideService(IUnitOfWork _unitOfWork, IMqttCommandService _mqttCommandService,
        IScooterTelemetryRepository _scooterTelemetryRepository, IZoneCacheService _zoneCacheService) : IRideService
    {
        public async Task<ActiveRideResponseDto> StartRideAsync(StartRideRequestDto request, Guid userId)
        {
            // 1. check if user has an active ride
            var rideRepo = _unitOfWork.GetRepository<Ride>();

            var activeRides = await rideRepo.GetEntityWithSpecAsync(new GetActiveRideByUserSpec(userId));

            if (activeRides is not null)
                throw new BadRequestException("You already have an active ride");

            // 2. check if the scooter exists
            var scooterRepo = _unitOfWork.GetRepository<Scooter>();

            var scooter = await _scooterTelemetryRepository.GetLatestTelemetryAsync(request.SerialNumber);

            if (scooter is null)
                throw new NotFoundException("Scooter", request.SerialNumber);

            var scooterInDatabase = await scooterRepo.GetEntityWithSpecAsync(new ScooterBySerialNumberSpecification(request.SerialNumber));

            if (scooterInDatabase is null)
                throw new NotFoundException("Scooter", request.SerialNumber);

            // 3. check if the scooter exists

            if (scooterInDatabase.Status != ScooterStatus.Available || scooter.BatteryLevel < 15)
                throw new BadRequestException("Can't Use this scooter");

            // 4. check if the user is close to the scooter

            var userLocation = new Point(new Coordinate(request.UserLongitude, request.UserLatitude)) { SRID = 4326 };

            var distanceBetweenUserAndScooter = CalculateDistanceInMeters(request.UserLatitude, request.UserLongitude, scooter.Latitude, scooter.Longitude);

            if (distanceBetweenUserAndScooter >= 50)
                throw new BadRequestException("You are too far from the scooter");

            // 5. check the active pricing tariff

            var tariffRepo = _unitOfWork.GetRepository<Tariff>();

            var tariff = await tariffRepo.GetEntityWithSpecAsync(new GetActiveTariffSpec());

            if (tariff is null)
                throw new BadRequestException("there is no Pricing tariffs available");

            // 6. start new ride
            var ride = new Ride()
            {
                UserId = userId,
                ScooterId = scooterInDatabase.Id,
                StartTime = DateTimeOffset.UtcNow,
                StartLocation = userLocation,
                AppliedUnlockFee = tariff.UnlockFee,
                AppliedPerMinuteRate = tariff.PerMinuteRate
            };

            scooterInDatabase.Status = ScooterStatus.InUse;

            scooterRepo.Update(scooterInDatabase);

            rideRepo.Add(ride);

            await _unitOfWork.SaveChangesAsync();

            // 7. Send Command to Start the scooter
            await _mqttCommandService.SendCommandAsync(scooter.SerialNumber, ScooterCommandType.StartScooter);

            // 8. return the new active ride
            var activeRideDto = ride.ToActiveRideDto();

            return activeRideDto;
        }

        public async Task<ActiveRideResponseDto> GetCurrentActiveRideAsync(Guid userId)
        {
            var rideRepo = _unitOfWork.GetRepository<Ride>();

            var activeRide = await rideRepo.GetEntityWithSpecAsync(new GetActiveRideByUserSpec(userId));

            if (activeRide is null)
                throw new NotFoundException("Active Ride", userId);

            var activeRideDto = activeRide.ToActiveRideDto();

            return activeRideDto;
        }

        public async Task<RideDto> EndRideAsync(EndRideRequestDto request, Guid userId)
        {
            var rideRepo = _unitOfWork.GetRepository<Ride>();

            var activeRide = await rideRepo.GetEntityWithSpecAsync(new GetActiveRideByUserSpec(userId));

            if (activeRide is null)
                throw new NotFoundException("Active Ride", userId);

            var zones = _zoneCacheService.GetZonesForPoint(request.UserLongitude, request.UserLatitude);

            if (!zones.Any())
                throw new BadRequestException("You cannot park outside the operational area. Please return to a valid zone.");

            foreach (var zone in zones)
                if (Enum.Parse<ZoneType>(zone.Type) == ZoneType.NoParking)
                    throw new BadRequestException("You are in a No-Parking zone. Please move the scooter to end your ride.");

            activeRide.EndTime = DateTimeOffset.UtcNow;

            activeRide.EndLocation = new Point(new Coordinate(request.UserLongitude, request.UserLatitude)) { SRID = 4326 };

            activeRide.EndPhotoUrl = request.EndPhotoUrl;

            activeRide.Status = RideStatus.Completed;

            decimal durationMinutes = Math.Round((decimal)(activeRide.EndTime.Value - activeRide.StartTime).TotalMinutes, 2);

            decimal totalCost = Math.Round(activeRide.AppliedUnlockFee + (durationMinutes * activeRide.AppliedPerMinuteRate), 2);

            activeRide.DurationMinutes = durationMinutes;

            activeRide.TotalCost = totalCost;

            activeRide.Scooter.Status = ScooterStatus.Available;

            activeRide.Scooter.Location = new Point(new Coordinate(request.UserLongitude, request.UserLatitude)) { SRID = 4326 };

            rideRepo.Update(activeRide);

            await _unitOfWork.SaveChangesAsync();

            await _mqttCommandService.SendCommandAsync(activeRide.Scooter.SerialNumber, ScooterCommandType.StopScooter);

            var rideDto = activeRide.ToDto();

            return rideDto;
        }

        private double CalculateDistanceInMeters(double userLatitude, double userLongitude, double scooterLatitude, double scooterLongitude)
        {
            var userLatitudeInRadian = userLatitude * (Math.PI / 180.0);
            var userLongitudeInRadian = userLongitude * (Math.PI / 180.0);
            var scooterLatitudeInRadian = scooterLatitude * (Math.PI / 180.0);
            var scooterLongitudeInRadian = scooterLongitude * (Math.PI / 180.0);

            var differenceLatitude = scooterLatitudeInRadian - userLatitudeInRadian;
            var differenceLongitude = scooterLongitudeInRadian - userLongitudeInRadian;

            var a = Math.Sin(differenceLatitude / 2) * Math.Sin(differenceLatitude / 2) + Math.Cos(userLatitudeInRadian) * Math.Cos(scooterLatitudeInRadian) * Math.Sin(differenceLongitude / 2) * Math.Sin(differenceLongitude / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var result = c * 6371000;

            return result;
        }
    }
}
