namespace ScooterRental.Service.Abstractions.AuthServices
{
    public interface ITokenService
    {
        Task<TokenDto> CreateTokenAsync(User user);

        string GenerateRefreshToken();
    }
}
