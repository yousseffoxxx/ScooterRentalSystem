namespace ScooterRental.Service.Mappings
{
    public static class AuthMappingExtensions
    {
        // 1. RegisterDto -> User
        public static User ToEntity(this RegisterDto dto, string idPhotoUrl)
        {
            return new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IdPhotoUrl = idPhotoUrl
            };
        }
        // 2. UpdateProfileDto -> User
        public static void UpdateEntity(this UpdateProfileDto dto, User user, string? avatarPhotoUrl)
        {
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(avatarPhotoUrl))
                user.AvatarUrl = avatarPhotoUrl;
        }
        // 3. User -> UserResponseDto
        public static UserResponseDto ToDto(this User user, string baseUrl)
        {
            string? formattedAvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)  
                ? null : $"{baseUrl.TrimEnd('/')}/{user.AvatarUrl.TrimStart('/')}";

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                AvatarUrl = formattedAvatarUrl,
                IdVerificationStatus = user.IdVerificationStatus.ToString(),
                AccountStatus = user.AccountStatus.ToString(),
                WalletBalance = user.Wallet?.Balance ?? 0.00m,
                PhoneVerified = user.PhoneNumberConfirmed
            };
        }
    }
}
