namespace ScooterRental.Domain.Models.Scooters
{
    public class ScooterAlert
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SerialNumber { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
