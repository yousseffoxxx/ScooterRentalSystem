namespace ScooterRental.Domain.Models
{
    public class ScooterModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Brand { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public decimal MaxSpeedKmH { get; set; }
        public decimal WeightLimitKg { get; set; }
        public int BatteryCapacityMah { get; set; }

        public ICollection<Scooter> Scooters { get; set; } = new List<Scooter>();
    }
}
