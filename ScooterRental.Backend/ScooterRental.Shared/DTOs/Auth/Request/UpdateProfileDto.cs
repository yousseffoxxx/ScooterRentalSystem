namespace ScooterRental.Shared.DTOs.Auth.Requests
{
    public record UpdateProfileDto(string? FullName, string? PhoneNumber, string? AvatarUrl)
    {

    }
}
