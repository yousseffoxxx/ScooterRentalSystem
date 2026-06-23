namespace ScooterRental.Service.Specifications
{
    public class ZoneByLocationSpecification : BaseSpecifications<Zone>
    {
        public ZoneByLocationSpecification(Point point) : base(zone => zone.Boundary.Contains(point))
        {
        }
    }
}
