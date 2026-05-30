namespace ScooterRental.Service.Mappings
{
    public static class AuthMappingExtensions
    {
        // 1. RegisterDto -> User
        public static User ToEntity(this RegisterDto dto, string idFrontPhotoUrl, string idBackPhotoUrl)
        {
            return new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IdFrontPhotoUrl = idFrontPhotoUrl,
                IdBackPhotoUrl = idBackPhotoUrl
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

        // 4. CreateAdminDto -> User
        public static User ToEntity(this CreateAdminDto dto)
        {
            return new User
            {
                FullName = dto.Name,
                Email = dto.Email,
                UserName = dto.Email,
            };
        }

        // 5. User -> AdminResponseDto
        public static AdminResponseDto ToDto(this User user)
        {
            return new AdminResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                AccountStatus = user.AccountStatus.ToString(),
            };
        }

        // 6. List of User -> List of UserResponseDto
        public static IReadOnlyList<UserResponseDto> ToDtoList(this IReadOnlyList<User> users, string baseUrl)
        {
            if (users == null || users.Count == 0)
                return new List<UserResponseDto>(0);

            var usersDtos = new List<UserResponseDto>(users.Count);
            foreach (var user in users)
            {
                string? formattedAvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)
                    ? null : $"{baseUrl.TrimEnd('/')}/{user.AvatarUrl.TrimStart('/')}";
                
                usersDtos.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    AvatarUrl = formattedAvatarUrl,
                    IdVerificationStatus = user.IdVerificationStatus.ToString(),
                    AccountStatus = user.AccountStatus.ToString(),
                    WalletBalance = user.Wallet.Balance,
                    PhoneVerified = user.PhoneNumberConfirmed
                });
            }
            return usersDtos;
        }
    }
}
