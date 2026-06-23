namespace ScooterRental.Service.RideServices
{
    public class RideService(IUnitOfWork _unitOfWork, IMqttCommandService _mqttCommandService,
        IScooterTelemetryRepository _scooterTelemetryRepository, IZoneCacheService _zoneCacheService, UserManager<User> _userManager,
        INotificationService _notificationService, IActiveRideCacheRepository _activeRideCacheRepository,
        ILocalStorageService _localStorageService, IConfiguration _configuration, IRealTimeBroadcastService _broadcastService) : IRideService
    {
        private readonly string _baseUrl = _configuration.GetSection("Urls")["BaseUrl"] ?? string.Empty;
        
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

            if (distanceBetweenUserAndScooter >= 100)
                throw new BadRequestException($"You are too far from the scooter Distance calculated: {distanceBetweenUserAndScooter} meters");

            // 5. check the active pricing tariff

            var tariffRepo = _unitOfWork.GetRepository<Tariff>();

            var tariff = await tariffRepo.GetEntityWithSpecAsync(new GetActiveTariffSpec());

            if (tariff is null)
                throw new BadRequestException("there is no Pricing tariffs available");

            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null || user.Wallet is null)
                throw new UnAuthorizedException("User or Wallet Not Found");

            if (user.Wallet.Balance < tariff.UnlockFee)
                throw new BadRequestException("Insufficient wallet balance to unlock the scooter.");

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

            var cacheModel = new ActiveRideCacheModel(ride.Id, user.Id, scooter.SerialNumber, user.FcmToken);

            await _activeRideCacheRepository.SetActiveRideAsync(cacheModel);

            if (!string.IsNullOrEmpty(user.FcmToken))
                await _notificationService.SendNotificationAsync(user.FcmToken,
                    "Ride Started",
                    "Your scooter is unlocked. Ride safely!");

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

            var savedParkingPhotoUrl = await _localStorageService.SaveFileAsync(request.EndPhoto, "uploads/parkingPhotos");

            activeRide.EndTime = DateTimeOffset.UtcNow;

            activeRide.EndLocation = new Point(new Coordinate(request.UserLongitude, request.UserLatitude)) { SRID = 4326 };

            activeRide.EndPhotoUrl = savedParkingPhotoUrl;

            activeRide.Status = RideStatus.Completed;

            decimal durationMinutes = Math.Ceiling((decimal)(activeRide.EndTime.Value - activeRide.StartTime).TotalMinutes);

            if (durationMinutes < 1)
                durationMinutes = 1;

            decimal totalCost = Math.Round(activeRide.AppliedUnlockFee + (durationMinutes * activeRide.AppliedPerMinuteRate), 2);

            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null || user.Wallet is null)
                throw new UnAuthorizedException("User or Wallet Not Found");

            user.Wallet.Balance -= totalCost;
            user.Wallet.TotalSpent += totalCost;

            activeRide.DurationMinutes = durationMinutes;
            activeRide.TotalCost = totalCost;
            activeRide.Scooter.Status = ScooterStatus.Available;
            activeRide.Scooter.Location = new Point(new Coordinate(request.UserLongitude, request.UserLatitude)) { SRID = 4326 };

            rideRepo.Update(activeRide);

            var walletTransaction = new WalletTransaction
            {
                WalletId = user.Wallet.Id,
                Amount = totalCost,
                Type = TransactionType.RidePayment,
                ReferenceId = activeRide.Id.ToString(),
                Description = $"Payment for ride on scooter {activeRide.Scooter.SerialNumber}"
            };

            _unitOfWork.GetRepository<WalletTransaction>().Add(walletTransaction);

            await _unitOfWork.SaveChangesAsync();

            await _broadcastService.BroadcastWalletTopUpToRiderAsync(userId.ToString(), user.Wallet.Balance);

            await _broadcastService.BroadcastNewParkingPhotoToAdminsAsync(activeRide.Id, activeRide.Scooter.SerialNumber, savedParkingPhotoUrl);

            await _activeRideCacheRepository.RemoveActiveRideAsync(activeRide.Scooter.SerialNumber);

            await _mqttCommandService.SendCommandAsync(activeRide.Scooter.SerialNumber, ScooterCommandType.StopScooter);

            if (!string.IsNullOrEmpty(user.FcmToken))
                await _notificationService.SendNotificationAsync(user.FcmToken,
                "Ride Completed",
                $"Your ride cost {totalCost} EGP. Remaining balance: {user.Wallet.Balance} EGP.");

            var rideDto = activeRide.ToDto();

            return rideDto;
        }

        public async Task<PaginatedResult<PendingParkingPhotoDto>> GetPendingParkingPhotosAsync(QueryParams queryParams)
        {
            var specifications = new PendingParkingPhotosSpecification(queryParams.PageIndex, queryParams.PageSize);

            var rides = await _unitOfWork.GetRepository<Ride>().GetAllWithSpecAsync(specifications);

            var totalCount = await _unitOfWork.GetRepository<Ride>().CountAsync(specifications);

            var ridesDtos = rides.ToParkingPhotoDtoList();

            return new PaginatedResult<PendingParkingPhotoDto>(queryParams.PageIndex, queryParams.PageSize, totalCount, ridesDtos);
        }

        public async Task ReviewParkingPhotoAsync(Guid rideId, ReviewParkingPhotoDto dto)
        {
            var rideRepo = _unitOfWork.GetRepository<Ride>();

            var ride = await rideRepo.GetEntityWithSpecAsync(new GetRideByIdSpecification(rideId));

            if (ride is null)
                throw new NotFoundException("Ride", rideId);

            if (!dto.IsApproved && dto.PenaltyAmount > 0)
            {
                ride.ParkingPhotoStatus = ReviewStatus.Rejected;
                ride.ParkingRejectionReason = dto.RejectionReason;
                ride.User.Wallet.Balance -= dto.PenaltyAmount;
                ride.User.Wallet.TotalSpent += dto.PenaltyAmount;

                var transaction = new WalletTransaction()
                {
                    WalletId = ride.User.Wallet.Id,
                    Amount = dto.PenaltyAmount,
                    Type = TransactionType.AdminAdjustment,
                    ReferenceId = ride.Scooter.SerialNumber
                };

                if (!string.IsNullOrEmpty(ride.User.FcmToken))
                    await _notificationService.SendNotificationAsync(ride.User.FcmToken,
                   "Warning: Bad Parking. You have been fined.",
                    $"Your Penalty cost {dto.PenaltyAmount} EGP. Remaining balance: {ride.User.Wallet.Balance} EGP.");

                await _unitOfWork.SaveChangesAsync();

                await _broadcastService.BroadcastWalletTopUpToRiderAsync(ride.UserId.ToString(), ride.User.Wallet.Balance);

                return;
            }

            ride.ParkingPhotoStatus = ReviewStatus.Approved;

            if (!string.IsNullOrEmpty(ride.User.FcmToken))
                await _notificationService.SendNotificationAsync(ride.User.FcmToken,
               "Thanks for parking safely!",
                "Thanks for parking safely!");
            return;
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

        public async Task<PaginatedResult<RideDto>> GetAllRidesAsync(QueryParams queryParams)
        {
            var specifications = new AllRidesSpecifications(queryParams.PageIndex, queryParams.PageSize);

            var rides = await _unitOfWork.GetRepository<Ride>().GetAllWithSpecAsync(specifications);

            var totalCount = await _unitOfWork.GetRepository<Ride>().CountAsync(specifications);

            var ridesDtos = rides.ToDtoList();

            return new PaginatedResult<RideDto>(queryParams.PageIndex, queryParams.PageSize, totalCount, ridesDtos);
        }
    }
}
