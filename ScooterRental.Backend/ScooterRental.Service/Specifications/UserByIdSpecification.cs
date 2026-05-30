namespace ScooterRental.Service.Specifications
{
    public class UserByIdSpecification : BaseSpecifications<User>
    {
        public UserByIdSpecification(Guid id) : base(u=>u.Id == id)
        {
            AddInclude(u => u.Wallet);

            AddInclude(u => u.Rides);
        }
    }
}
