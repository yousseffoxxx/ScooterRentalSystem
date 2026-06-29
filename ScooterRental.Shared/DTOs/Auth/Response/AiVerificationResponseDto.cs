namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record AiVerificationResponseDto
    {
        public bool Success { get; init; }
        public bool Valid { get; init; }
        public bool NeedsManualId { get; init; }
        public string? Error { get; init; }
        public AiDataDto Data { get; init; } = null!;
    }
}
