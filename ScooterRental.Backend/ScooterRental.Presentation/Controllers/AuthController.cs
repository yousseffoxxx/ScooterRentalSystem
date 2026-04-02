namespace ScooterRental.Presentation.Controllers
{
    // BaseUrl/api/Auth/
    public class AuthController(IServiceManager _serviceManager) : ApiController
    {

        [HttpPost("register")]
        public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _serviceManager.AuthService.RegisterAsync(registerDto);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _serviceManager.AuthService.LoginAsync(loginDto);

            return Ok(result);
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<MessageResponseDto>> VerifyEmail([FromBody] VerifyOtpDto verifyOtpDto)
        {
            await _serviceManager.AuthService.VerifyOtpAsync(verifyOtpDto);

            return Ok(new MessageResponseDto("Email verified successfully."));
        }

        [HttpPost("resend-otp")]
        public async Task<ActionResult<MessageResponseDto>> ResendOtp([FromBody] ResendOtpDto resendOtpDto)
        {
            await _serviceManager.AuthService.ResendOtpAsync(resendOtpDto);

            return Ok(new MessageResponseDto("A new code has been sent."));
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<MessageResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _serviceManager.AuthService.ForgotPasswordAsync(forgotPasswordDto);

            return Ok(new MessageResponseDto(result));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<MessageResponseDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            await _serviceManager.AuthService.ResetPasswordAsync(resetPasswordDto);

            return Ok(new MessageResponseDto("Password reset successfully."));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserResponseDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _serviceManager.AuthService.GetProfileAsync(userId);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _serviceManager.AuthService.UpdateProfileAsync(userId, updateProfileDto);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<MessageResponseDto>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _serviceManager.AuthService.ChangePasswordAsync(userId, changePasswordDto);

            return Ok(new MessageResponseDto("Password changed successfully."));
        }
    }
}
