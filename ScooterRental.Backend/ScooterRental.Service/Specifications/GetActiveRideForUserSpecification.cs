namespace ScooterRental.Service.Specifications
{
    public class GetActiveRideForUserSpecification : BaseSpecifications<Ride>
    {
        public GetActiveRideForUserSpecification(string serialNumber) : base(r => r.Scooter.SerialNumber == serialNumber && r.Status == RideStatus.Active)
        {
            AddInclude(r => r.User);
        }
    }
}
