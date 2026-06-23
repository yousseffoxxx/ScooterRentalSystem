namespace ScooterRental.Shared.DTOs.AdminManagement.Users.Response
{
    public record AdminResponseDto
    {
        public required Guid Id { get; init; }
        public required string FullName { get; init; }
        public required string Email { get; init; }
        public required string AccountStatus { get; init; } 

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
