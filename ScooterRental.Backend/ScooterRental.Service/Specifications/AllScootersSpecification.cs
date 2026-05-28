namespace ScooterRental.Service.Specifications
{
    public class AllScootersSpecification : BaseSpecifications<Scooter>
    {
        public AllScootersSpecification(int pageIndex, int pageSize)
        {
            AddInclude(s => s.Model);

            ApplyPagination(pageIndex, pageSize);
        }

        public AllScootersSpecification() : base(s => s.Status == ScooterStatus.Available)
        {
            AddInclude(s => s.Model);
        }
    }
}
