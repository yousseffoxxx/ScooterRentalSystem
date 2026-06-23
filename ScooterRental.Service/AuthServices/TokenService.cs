namespace ScooterRental.Service.AuthServices
{
    public class TokenService(IOptions<JwtSettings> _options, UserManager<User> _userManager) : ITokenService
    {
        public async Task<TokenDto> CreateTokenAsync(User user)
        {
            var jwtOptions = _options.Value;

            // User Defined Claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

            var signingCreds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var tokenExpiration = now.AddMinutes(jwtOptions.AccessTokenExpirationMinutes);
            var refreshExpiration = now.AddDays(jwtOptions.RefreshTokenExpirationDays);

            var token = new JwtSecurityToken(
                    audience: jwtOptions.Audience,
                    issuer: jwtOptions.Issuer,
                    expires: tokenExpiration,
                    claims: authClaims,
                    signingCredentials: signingCreds
                    );

            return new TokenDto(
                AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt: tokenExpiration,
                RefreshToken: GenerateRefreshToken(),
                RefreshTokenExpiration: refreshExpiration);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
