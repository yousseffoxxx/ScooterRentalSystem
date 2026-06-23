namespace ScooterRental.Service.Specifications
{
    public class AllUsersSpecifications : BaseSpecifications<User>
    {
        public AllUsersSpecifications(int pageIndex, int pageSize)
        {
            AddInclude(u => u.Wallet);

            AddInclude(u => u.Rides);

            ApplyPagination(pageIndex, pageSize);
        }
    }
}
