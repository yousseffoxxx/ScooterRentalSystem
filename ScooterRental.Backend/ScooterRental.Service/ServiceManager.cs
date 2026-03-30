namespace ScooterRental.Service
{
    public class ServiceManager(UserManager<User> _userManager, ITokenService _tokenService, IConfiguration _configuration, IOtpService _otpService, IEmailService _emailService) : IServiceManager
    {
        private readonly Lazy<IAuthService> _lazyAuthService = new Lazy<IAuthService>(() => new AuthService(_userManager, _tokenService, _configuration, _otpService, _emailService));
        public IAuthService AuthService => _lazyAuthService.Value;

    }
}
