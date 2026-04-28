namespace ScooterRental.Service.Specifications
{
    public class ZoneByIdSpecification : BaseSpecifications<Zone>
    {
        public ZoneByIdSpecification(Guid id) : base(x => x.Id == id)
        {
        }
    }
}
