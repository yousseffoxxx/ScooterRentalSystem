namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record TokenDto(string AccessToken, DateTime ExpiresAt, string RefreshToken, DateTime RefreshTokenExpiration);
}
