namespace ScooterRental.Service.Specifications
{
    public class AllTariffsSpecification : BaseSpecifications<Tariff>
    {
        public AllTariffsSpecification(int pageIndex, int pageSize)
        {
            ApplyPagination(pageIndex, pageSize);
        }
    }
}
