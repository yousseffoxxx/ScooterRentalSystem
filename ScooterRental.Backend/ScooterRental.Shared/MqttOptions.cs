namespace ScooterRental.Shared
{
    public record MqttOptions
    {
        public string BrokerAddress { get; init; } = string.Empty;
        public int Port { get; init; }
        public string ClientId { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
    }
}
