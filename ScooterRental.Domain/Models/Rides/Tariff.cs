namespace ScooterRental.Domain.Models.Rides
{
    public class Tariff
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal UnlockFee { get; set; } = 0.00M;
        public decimal PerMinuteRate { get; set; } = 0.00M;
        public bool IsActive { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
