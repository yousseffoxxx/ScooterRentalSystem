namespace ScooterRental.Domain.Models
{
    public record ScooterTelemetry
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int BatteryLevel { get; init; }
        public DateTimeOffset Timestamp { get; init; }
    }
}
