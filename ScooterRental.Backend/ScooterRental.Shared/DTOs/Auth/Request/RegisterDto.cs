namespace ScooterRental.Shared.DTOs.Auth.Requests
{
    public record RegisterDto
    {
        public required string Name { get; init; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        public required string Password { get; init; }
        public required string IdPhotoUrl { get; init; }
    }
}
