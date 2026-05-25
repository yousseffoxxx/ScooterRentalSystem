namespace ScooterRental.Shared
{
    public record PaymobOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string HMAC { get; set; } = string.Empty;
        public string WalletIntegrationId { get; set; } = string.Empty;
    }
}
