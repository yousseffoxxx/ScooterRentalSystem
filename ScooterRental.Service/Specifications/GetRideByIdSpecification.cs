namespace ScooterRental.Service.Specifications
{
    public class GetRideByIdSpecification : BaseSpecifications<Ride>
    {
        public GetRideByIdSpecification(Guid rideId) : base(r => r.Id == rideId)
        {
            AddInclude(r => r.Scooter);
            AddInclude(r => r.User);
            AddInclude(r => r.User.Wallet);
        }
    }
}
