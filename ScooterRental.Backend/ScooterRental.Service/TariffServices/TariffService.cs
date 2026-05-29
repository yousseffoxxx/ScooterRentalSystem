namespace ScooterRental.Service.TariffServices
{
    public class TariffService(IUnitOfWork _unitOfWork) : ITariffService
    {
        public async Task<PaginatedResult<TariffDto>> GetAllTariffsAsync(QueryParams queryParams)
        {
            var specification = new AllTariffsSpecification(queryParams.PageIndex, queryParams.PageSize);

            var tariffs = await _unitOfWork.GetRepository<Tariff>().GetAllWithSpecAsync(specification);

            var tariffsTotalCount = await _unitOfWork.GetRepository<Tariff>().CountAsync(specification);

            var tariffsDtos = tariffs.ToDtoList();

            return new PaginatedResult<TariffDto>(queryParams.PageIndex, queryParams.PageSize, tariffsTotalCount, tariffsDtos);
        }

        public async Task<TariffDto> GetActiveTariffAsync()
        {
            var tariff = await _unitOfWork.GetRepository<Tariff>().GetEntityWithSpecAsync(new GetActiveTariffSpec());

            if (tariff is null)
                throw new BadRequestException("there Is no Active Tariffs Available");

            var tariffDto = tariff.ToDto();

            return tariffDto;
        }

        public async Task<TariffDto> CreateTariffAsync(TariffForCreationDto dto)
        {
            var tariff = dto.ToEntity();

            _unitOfWork.GetRepository<Tariff>().Add(tariff);

            await _unitOfWork.SaveChangesAsync();

            return tariff.ToDto();
        }

        public async Task<bool> ActivateTariffAsync(Guid id)
        {
            var tariff = await _unitOfWork.GetRepository<Tariff>().GetByIdAsync(id);

            if (tariff is null)
                throw new NotFoundException("Tariff", id);

            var activeTariff = await _unitOfWork.GetRepository<Tariff>().GetEntityWithSpecAsync(new GetActiveTariffSpec());
            
            if (activeTariff != null)
            {
                if (tariff.Id == activeTariff.Id)
                    return true; // It's already the active one, do nothing.

                activeTariff.IsActive = false;
                _unitOfWork.GetRepository<Tariff>().Update(activeTariff);
            }

            tariff.IsActive = true;

            _unitOfWork.GetRepository<Tariff>().Update(tariff);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTariffAsync(Guid id)
        {
            var tariff = await _unitOfWork.GetRepository<Tariff>().GetByIdAsync(id);

            if (tariff is null)
                throw new NotFoundException("Tariff", id);
            
            if (tariff.IsActive)
                throw new BadRequestException("You cannot delete the currently active tariff. Please activate another tariff first.");
            
            _unitOfWork.GetRepository<Tariff>().Delete(tariff);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
