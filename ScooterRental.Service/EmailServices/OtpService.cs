namespace ScooterRental.Service.EmailServices
{
    public class OtpService(UserManager<User> _userManager, IEmailService _emailService) : IOtpService
    {
        public async Task<bool> SendOtpAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new BadRequestException("Cannot send OTP: User does not have a registered email address.");

            var otpCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            var subject = "Your Scooter Rental Verification Code";
            
            var htmlMessage = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; text-align: center;'>
                    <h2>Welcome to Scooter Rental!</h2>
                    <p>Your verification code is:</p>
                    <h1 style='color: #007bff; letter-spacing: 5px;'>{otpCode}</h1>
                    <p style='color: gray; font-size: 12px;'>This code will expire in a few minutes. Please do not share it with anyone.</p>
                </div>";

            await _emailService.SendEmailAsync(user.Email, subject, htmlMessage);
            
            return true;
        }
        public async Task<bool> VerifyOtpAsync(User user, string code) 
            => await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
    }
}
