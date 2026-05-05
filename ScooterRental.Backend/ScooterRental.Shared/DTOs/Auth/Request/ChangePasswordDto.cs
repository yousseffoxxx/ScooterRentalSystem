namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record ChangePasswordDto(string CurrentPassword, string NewPassword)
    {

    }
}
