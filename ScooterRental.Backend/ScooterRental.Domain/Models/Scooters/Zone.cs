namespace ScooterRental.Domain.Models.Scooters
{
    public class Zone
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ZoneType Type { get; set; }
        public double? SpeedLimitKmH { get; set; }
        public bool IsActive { get; set; } = true;
        public Polygon Boundary { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
