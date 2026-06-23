namespace ScooterRental.Service.Specifications
{
    public class GetActiveTariffSpec : BaseSpecifications<Tariff>
    {
        public GetActiveTariffSpec() : base(t => t.IsActive)
        {
            AddOrderByDescending(t => t.CreatedAt);
        }
    }
}
