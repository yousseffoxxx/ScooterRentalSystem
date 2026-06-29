namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record RegisterDto
    {
        public required string FullName { get; init; }
        public required string PhoneNumber { get; init; }
        public required string Password { get; init; }
        public string? Email { get; init; }
        public string? ManualNationalId { get; init; }
        public required string FirebaseToken { get; init; }
        public required IFormFile IdFrontPhoto { get; init; }
        public required IFormFile IdBackPhoto { get; init; }
        public required IFormFile SelfiePhoto { get; init; }
    }
}
