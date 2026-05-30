namespace ScooterRental.Service.Abstractions.AuthServices
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<bool> ResendOtpAsync(ResendOtpDto resendOtpDto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<UserResponseDto> GetProfileAsync(string userId);
        Task<UserResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
        Task<bool> RemoveDeadFcmTokenAsync(string fcmToken);
        Task<bool> UpdateFcmTokenAsync(string userId, UpdateFcmTokenDto tokenDto);
        Task<AdminResultDto> CreateAdminAsync(CreateAdminDto createAdminDto, string secretKey);
        Task<PaginatedResult<UserResponseDto>> GetAllUsersAsync(QueryParams queryParams);
        Task<UserResponseDto> GetUserByIdAsync(Guid id);
        Task<bool> SuspendUserAsync(Guid id);
        Task<bool> ActivateUserAsync(Guid id);
    }
}
