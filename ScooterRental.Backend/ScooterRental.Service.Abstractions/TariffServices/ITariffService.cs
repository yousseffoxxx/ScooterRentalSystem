namespace ScooterRental.Service.Abstractions.TariffServices
{
    public interface ITariffService
    {
        Task<TariffDto> CreateTariffAsync(TariffForCreationDto dto);
        Task<PaginatedResult<TariffDto>> GetAllTariffsAsync(QueryParams queryParams);
        Task<TariffDto> GetActiveTariffAsync();
        Task<bool> ActivateTariffAsync(Guid id);
        Task<bool> DeleteTariffAsync(Guid id);
    }
}
