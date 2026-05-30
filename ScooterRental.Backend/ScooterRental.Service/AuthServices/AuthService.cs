namespace ScooterRental.Service.AuthServices
{
    public class AuthService(UserManager<User> _userManager, ITokenService _tokenService,
        IConfiguration _configuration, IOtpService _otpService, IEmailService _emailService, ILocalStorageService _localStorageService,
        IUnitOfWork _unitOfWork) : IAuthService
    {
        private readonly string _baseUrl = _configuration.GetSection("Urls")["BaseUrl"] ?? string.Empty;

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var savedFrontPhotoUrl = await _localStorageService.SaveFileAsync(registerDto.IdFrontPhoto, "uploads/ids");
            var savedBackPhotoUrl = await _localStorageService.SaveFileAsync(registerDto.IdBackPhoto, "uploads/ids");

            var user = registerDto.ToEntity(savedFrontPhotoUrl, savedBackPhotoUrl);

            user.Wallet = new Wallet
            {
                Balance = 0,
                HeldAmount = 0,
                TotalSpent = 0,
                TotalToppedUp = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            await _userManager.AddToRoleAsync(user, "Customer");

            await _otpService.SendOtpAsync(user);

            var token = await _tokenService.CreateTokenAsync(user);

            var userDto = user.ToDto(_baseUrl);

            return new AuthResultDto(userDto, token);
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null)
                throw new UnAuthorizedException("Invalid Email or Password");

            if (!user.EmailConfirmed)
                throw new UnAuthorizedException("Please verify your email address before logging in.");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnAuthorizedException("Your account is temporarily locked due to multiple failed login attempts. Please try again later.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordValid)
            {
                // 4. Record the failed attempt in the database
                await _userManager.AccessFailedAsync(user);

                // Optional UX: If that specific wrong guess just triggered the lockout, tell them immediately!
                if (await _userManager.IsLockedOutAsync(user))
                    throw new UnAuthorizedException("Too many failed attempts. Your account is now locked.");

                throw new UnAuthorizedException("Invalid Email or Password");
            }

            // 5. Success! Reset the failed attempt counter back to 0.
            await _userManager.ResetAccessFailedCountAsync(user);

            var token = await _tokenService.CreateTokenAsync(user);
            var userDto = user.ToDto(_baseUrl);

            return new AuthResultDto(userDto, token);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);

            if (user is null)
                throw new NotFoundException("User", verifyOtpDto.Email);

            var isValid = await _otpService.VerifyOtpAsync(user, verifyOtpDto.Code);

            if (!isValid)
                throw new BadRequestException("Invalid or expired OTP code.");

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            return true;
        }

        public async Task<bool> ResendOtpAsync(ResendOtpDto resendOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(resendOtpDto.Email);

            if (user is null)
                throw new NotFoundException("User", resendOtpDto.Email);

            if (user.EmailConfirmed)
                throw new BadRequestException("This email is already verified. You can log in.");

            await _otpService.SendOtpAsync(user);

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

            string? savedPhotoUrl = null;

            if (updateProfileDto.AvatarPhoto != null && updateProfileDto.AvatarPhoto.Length > 0)
                savedPhotoUrl = await _localStorageService.SaveFileAsync(updateProfileDto.AvatarPhoto, "uploads/avatars");


            updateProfileDto.UpdateEntity(user, savedPhotoUrl);

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

            var encodedToken = Uri.EscapeDataString(token);

            var resetLink = $"{_baseUrl}/reset-password?email={user.Email}&token={encodedToken}";

            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

            return "Password reset email sent successfully.";
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user is null)
                throw new NotFoundException("User", resetPasswordDto.Email);

            var decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            return true;
        }

        public async Task<bool> RemoveDeadFcmTokenAsync(string fcmToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.FcmToken == fcmToken);

            if (user is null)
                return false;

            user.FcmToken = null;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> UpdateFcmTokenAsync(string userId, UpdateFcmTokenDto tokenDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User", userId);

            user.FcmToken = tokenDto.Token;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<AdminResultDto> CreateAdminAsync(CreateAdminDto createAdminDto, string secretKey)
        {
            var configuredSecret = _configuration.GetSection("AdminSettings")["CreationKey"];

            if (string.IsNullOrEmpty(configuredSecret) || secretKey != configuredSecret)
                throw new UnAuthorizedException("Invalid admin creation secret.");

            var user = createAdminDto.ToEntity();

            var result = await _userManager.CreateAsync(user, createAdminDto.Password);

            if (!result.Succeeded)
                throw CreateValidationException(result);

            await _userManager.AddToRoleAsync(user, "Admin");

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _userManager.ConfirmEmailAsync(user, emailToken);

            var token = await _tokenService.CreateTokenAsync(user);

            var adminDto = user.ToDto();

            return new AdminResultDto(adminDto, token);
        }

        public async Task<PaginatedResult<UserResponseDto>> GetAllUsersAsync(QueryParams queryParams)
        {
            var users = await _unitOfWork.GetRepository<User>()
                .GetAllWithSpecAsync(new AllUsersSpecifications(queryParams.PageIndex, queryParams.PageSize));

            var usersTotalCount = await _userManager.Users.CountAsync();

            var userDtos = users.ToDtoList(_baseUrl);

            return new PaginatedResult<UserResponseDto>(queryParams.PageIndex, users.Count, usersTotalCount, userDtos);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.GetRepository<User>().GetEntityWithSpecAsync(new UserByIdSpecification(id));

            if(user is null)
                throw new NotFoundException("User", id);

            var userDto = user.ToDto(_baseUrl);

            return userDto;
        }

        public async Task<bool> SuspendUserAsync(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                throw new NotFoundException("User", id);

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<bool> ActivateUserAsync(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                throw new NotFoundException("User", id);

            await _userManager.SetLockoutEndDateAsync(user, null);

            await _userManager.UpdateAsync(user);

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
