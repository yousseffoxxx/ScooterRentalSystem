namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record FaceVerification
    {
        public bool Verified { get; init; }
        public double Confidence { get; init; }

        [JsonPropertyName("match_level")]
        public string MatchLevel { get; init; } = string.Empty;
    }
}
