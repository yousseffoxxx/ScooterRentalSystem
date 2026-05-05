namespace ScooterRental.Service.Specifications
{
    public class ScooterBySerialNumberSpecification : BaseSpecifications<Scooter>
    {
        public ScooterBySerialNumberSpecification(string serialNumber) : base(s => s.SerialNumber == serialNumber)
        {
        }
    }
}
