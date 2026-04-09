namespace ScooterRental.Domain.Models
{
    public class Zone
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ZoneType Type { get; set; }
        public Polygon Boundary { get; set; } = null!;
        public decimal? SpeedLimitKmH { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
