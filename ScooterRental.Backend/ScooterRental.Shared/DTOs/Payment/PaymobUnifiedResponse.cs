namespace ScooterRental.Shared.DTOs.Payment
{
    public record PaymobUnifiedResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        // 1. Catch the standard redirect URL (if it exists)
        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; set; }

        // 2. Catch the Wallet iframe URL (if it exists)
        [JsonPropertyName("iframe_redirection_url")]
        public string? IframeRedirectionUrl { get; set; }

        [JsonIgnore]
        public string FinalUrl => IframeRedirectionUrl ?? RedirectUrl ?? "";

    }
}
