namespace ScooterRental.Domain.Models.Scooters
{
    public class ScooterTelemetry
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BatteryLevel { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public bool IsOutOfBounds { get; set; }
    }
}
