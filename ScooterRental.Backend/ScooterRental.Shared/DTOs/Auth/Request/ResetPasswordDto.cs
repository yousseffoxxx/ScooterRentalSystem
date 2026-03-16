namespace ScooterRental.Shared.DTOs.Auth.Requests
{
    public record ResetPasswordDto(string Email, string Token, string NewPassword)
    {

    }
}
