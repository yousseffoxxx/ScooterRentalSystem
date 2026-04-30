namespace ScooterRental.Domain.Models.Scooters
{
    public class Scooter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SerialNumber { get; set; } = string.Empty;
        public int CurrentBatteryLevel { get; set; } // 0 to 100
        public ScooterStatus Status { get; set; } = ScooterStatus.Available;
        public Point? Location { get; set; }
        public DateTimeOffset LastPingAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid ModelId { get; set; }
        public ScooterModel Model { get; set; } = null!;
        public ICollection<Ride> Rides { get; set; } = new List<Ride>();
    }
}
