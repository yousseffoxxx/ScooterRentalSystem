namespace ScooterRental.Service.Specifications
{
    public class GetActiveRideByUserSpec : BaseSpecifications<Ride>
    {
        public GetActiveRideByUserSpec(Guid userId) : base(r => r.UserId == userId && r.Status == RideStatus.Active)
        {
            AddInclude(r => r.Scooter);
        }
    }
}
