namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record RegisterDto
    {
        public required string FullName { get; init; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        public required string Password { get; init; }
        public required string IdPhotoUrl { get; init; }
    }
}
