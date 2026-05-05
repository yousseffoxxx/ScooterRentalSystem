namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record AuthResultDto(UserResponseDto User, TokenDto Token);
    
}
