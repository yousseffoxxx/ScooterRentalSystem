namespace ScooterRental.Shared.DTOs.Auth.Requests
{
    public record ChangePasswordDto(string CurrentPassword, string NewPassword)
    {

    }
}
