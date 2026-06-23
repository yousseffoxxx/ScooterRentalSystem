namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record UpdateProfileDto(string? FullName, string? PhoneNumber, IFormFile? AvatarPhoto)
    {

    }
}
