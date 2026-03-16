namespace ScooterRental.Shared.DTOs.Auth.Requests
{
    public record VerifyOtpDto(string PhoneNumber, string Otp)
    {
    }
}
