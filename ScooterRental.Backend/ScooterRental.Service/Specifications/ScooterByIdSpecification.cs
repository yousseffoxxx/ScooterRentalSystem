namespace ScooterRental.Service.Specifications
{
    public class ScooterByIdSpecification : BaseSpecifications<Scooter>
    {
        public ScooterByIdSpecification(Guid id) : base(s => s.Id == id)
        {
            AddInclude(s => s.Model);
        }
    }
}
