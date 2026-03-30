namespace ScooterRental.Shared.DTOs.Auth.Requests
{
    public record VerifyOtpDto(string Email, string Code)
    {
    }
}
