using ScooterRental.Shared.DTOs.Auth.Request;

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

    }
}
