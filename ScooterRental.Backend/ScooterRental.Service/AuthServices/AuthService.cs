namespace ScooterRental.Service.AuthServices
{
    public class AuthService(UserManager<User> _userManager, ITokenService _tokenService, IConfiguration _configuration) : IAuthService
    {
        private readonly string _baseUrl = _configuration.GetSection("Urls")["BaseUrl"] ?? string.Empty;

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = registerDto.ToEntity();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            await _userManager.AddToRoleAsync(user, "Customer");

            var token = await _tokenService.CreateTokenAsync(user);

            var userDto = user.ToDto(_baseUrl);

            return new AuthResultDto(userDto, token);
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null)
                throw new UnAuthorizedException("Invalid Email or Password");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
                throw new UnAuthorizedException("Invalid Email or Password");

            var token = await _tokenService.CreateTokenAsync(user);
            var userDto = user.ToDto(_baseUrl);

            return new AuthResultDto(userDto, token);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.PhoneNumber == verifyOtpDto.PhoneNumber);

            if (user is null)
                throw new NotFoundException("User", verifyOtpDto.PhoneNumber);

            user.PhoneNumberConfirmed = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            return true;
        }

        public async Task<UserResponseDto> GetProfileAsync(string userId)
        {
            var parsedId = Guid.Parse(userId);
            
            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == parsedId);

            if (user is null)
                throw new NotFoundException("User", userId);

            var userDto = user.ToDto(_baseUrl);

            return userDto;
        }

        public async Task<UserResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto)
        {
            var parsedId = Guid.Parse(userId);

            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == parsedId);

            if (user is null)
                throw new NotFoundException("User", userId);

            updateProfileDto.UpdateEntity(user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            var userDto = user.ToDto(_baseUrl);

            return userDto;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User", userId);

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            return true;
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user is null)
                throw new NotFoundException("User", forgotPasswordDto.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return token;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user is null)
                throw new NotFoundException("User", resetPasswordDto.Email);

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            return true;
        }

        private static AppValidationException CreateValidationException(IdentityResult result)
        {
            var errorDictionary = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray()
                );

            return new AppValidationException(errorDictionary);
        }
    }
}
