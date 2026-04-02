namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record VerifyOtpDto(string Email, string Code)
    {
    }
}
