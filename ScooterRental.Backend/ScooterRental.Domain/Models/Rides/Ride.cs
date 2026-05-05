namespace ScooterRental.Domain.Models.Rides
{
    public class Ride
    {
        public Guid Id { get; set; }
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
        public Point StartLocation { get; set; } = null!;
        public decimal AppliedUnlockFee { get; set; } = 0.00M;
        public decimal AppliedPerMinuteRate { get; set; } = 0.00M;
        public RideStatus Status { get; set; } = RideStatus.Active;
        public DateTimeOffset? EndTime { get; set; }
        public Point? EndLocation { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? DurationMinutes { get; set; }
        public string? EndPhotoUrl { get; set; }

        public User User { get; set; } = null!;
        public Scooter Scooter { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid ScooterId { get; set; }
    }
}
