namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record ResetPasswordDto(string Email, string Token, string NewPassword)
    {

    }
}
