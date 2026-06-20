namespace ScooterRental.Service.Specifications
{
    public class AllRidesSpecifications : BaseSpecifications<Ride>
    {
        public AllRidesSpecifications(int pageIndex, int pageSize)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Scooter);
            AddOrderByDescending(r => r.StartTime);
            ApplyPagination(pageIndex, pageSize);
        }
    }
}
