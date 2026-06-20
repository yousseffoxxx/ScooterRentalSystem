namespace ScooterRental.Service.Specifications
{
    public class PendingParkingPhotosSpecification : BaseSpecifications<Ride>
    {
        public PendingParkingPhotosSpecification(int pageIndex, int pageSize) : base(r => r.ParkingPhotoStatus == ReviewStatus.Pending && r.EndPhotoUrl != null)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Scooter);
            AddOrderByDescending(r => r.StartTime);
            ApplyPagination(pageIndex, pageSize);
        }
    }
}
