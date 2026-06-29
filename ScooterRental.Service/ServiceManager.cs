namespace ScooterRental.Service
{
    public class ServiceManager(UserManager<User> _userManager, ITokenService _tokenService, IConfiguration _configuration,
        ILocalStorageService _localStorageService, IUnitOfWork _unitOfWork, IValidator<ZoneForCreationDto> _createValidator,
        IValidator<ZoneForUpdateDto> _updateValidator, IRedisZoneEventPublisher _redisZoneEventPublisher, IMqttCommandService _mqttCommandService,
        IScooterTelemetryRepository _scooterTelemetryRepository, IZoneCacheService _zoneCacheService, IHttpClientFactory _httpClientFactory,
        IOptions<PaymobOptions> _options, INotificationService _notificationService, IActiveRideCacheRepository _activeRideCacheRepository,
        IAiVerificationService _aiVerificationService, IRealTimeBroadcastService _broadcastService, IEncryptionService _encryptionService)
        : IServiceManager
    {
        private readonly Lazy<IAuthService> _lazyAuthService = new Lazy<IAuthService>(() => new AuthService(_userManager, _tokenService, _configuration, _localStorageService, _unitOfWork, _aiVerificationService, _encryptionService));
        public IAuthService AuthService => _lazyAuthService.Value;

        private readonly Lazy<IScooterService> _lazyScooterService = new Lazy<IScooterService>(() => new ScooterService(_unitOfWork, _mqttCommandService));
        public IScooterService ScooterService => _lazyScooterService.Value;

        private readonly Lazy<IZoneService> _lazyZoneService = new Lazy<IZoneService>(() => new ZoneService(_unitOfWork, _createValidator, _updateValidator, _redisZoneEventPublisher));
        public IZoneService ZoneService => _lazyZoneService.Value;

        private readonly Lazy<IRideService> _lazyRideService = new Lazy<IRideService>(() => new RideService(_unitOfWork, _mqttCommandService, _scooterTelemetryRepository, _zoneCacheService, _userManager, _notificationService, _activeRideCacheRepository, _localStorageService, _configuration, _broadcastService));
        public IRideService RideService => _lazyRideService.Value;

        private readonly Lazy<IPaymobService> _lazyPaymobService = new Lazy<IPaymobService>(() => new PaymobService(_httpClientFactory, _options, _userManager, _notificationService, _unitOfWork, _broadcastService));
        public IPaymobService PaymobService => _lazyPaymobService.Value;

        private readonly Lazy<ITariffService> _lazyTariffService = new Lazy<ITariffService>(() => new TariffService(_unitOfWork));
        public ITariffService TariffService => _lazyTariffService.Value;
    }
}
