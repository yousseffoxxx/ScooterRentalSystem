namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record AiVerificationResponseDto
    {
        public bool Success { get; init; }
        public bool Valid { get; init; }
        public string? Error { get; init; }
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("national_id")]
        public string NationalId { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string Governorate { get; init; } = string.Empty;

        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; init; } = string.Empty;

        [JsonPropertyName("face_verification")]
        public FaceVerification FaceVerification { get; init; } = null!;
    }
}
