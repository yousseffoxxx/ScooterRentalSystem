namespace ScooterRental.Service
{
    public class ServiceManager(UserManager<User> _userManager, ITokenService _tokenService, IConfiguration _configuration,
        IOtpService _otpService, IEmailService _emailService,IUnitOfWork _unitOfWork,IValidator<ZoneForCreationDto> _createValidator,
        IValidator<ZoneForUpdateDto> _updateValidator,IRedisZoneEventPublisher _redisZoneEventPublisher) 
        : IServiceManager
    {
        private readonly Lazy<IAuthService> _lazyAuthService = new Lazy<IAuthService>(() => new AuthService(_userManager, _tokenService, _configuration, _otpService, _emailService));
        public IAuthService AuthService => _lazyAuthService.Value;

        private readonly Lazy<IScooterService> _lazyScooterService = new Lazy<IScooterService>(() => new ScooterService(_unitOfWork));
        public IScooterService ScooterService => _lazyScooterService.Value;

        private readonly Lazy<IZoneService> _lazyZoneService = new Lazy<IZoneService>(() => new ZoneService(_unitOfWork,_createValidator,_updateValidator, _redisZoneEventPublisher));
        public IZoneService ZoneService => _lazyZoneService.Value;

    }
}
