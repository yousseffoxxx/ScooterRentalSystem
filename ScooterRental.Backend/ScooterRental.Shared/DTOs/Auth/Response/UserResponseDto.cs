namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record UserResponseDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        public required string? AvatarUrl { get; init; }
        public required string IdVerificationStatus { get; init; }
        public required string AccountStatus { get; init; }
        public required decimal WalletBalance { get; init; }
        public required bool PhoneVerified { get; init; }
    }
}
