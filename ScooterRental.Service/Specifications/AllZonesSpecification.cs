namespace ScooterRental.Service.Specifications
{
    public class AllZonesSpecification : BaseSpecifications<Zone>
    {
        public AllZonesSpecification(int pageIndex, int pageSize, bool? isActive) : base(x=>x.IsActive == isActive)
        {
            ApplyPagination(pageIndex, pageSize);
        }
        public AllZonesSpecification(bool? isActive) : base(x=>x.IsActive == isActive)
        {
        }

    }
}
