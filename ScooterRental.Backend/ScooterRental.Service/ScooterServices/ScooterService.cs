namespace ScooterRental.Service.ScooterServices
{
    public class ScooterService(IUnitOfWork _unitOfWork) : IScooterService
    {
        public async Task<PaginatedResult<ScooterDto>> GetAllScootersAsync(ScooterQueryParams queryParams)
        {
            var specifications = new AllScootersSpecification(queryParams.PageIndex, queryParams.PageSize);

            var scooters = await _unitOfWork.GetRepository<Scooter>().GetAllWithSpecAsync(specifications);

            var scootersTotalCount = await _unitOfWork.GetRepository<Scooter>().CountAsync(specifications);

            var scooterDtos = scooters.ToDtoList();

            return new PaginatedResult<ScooterDto>(queryParams.PageIndex, scooters.Count, scootersTotalCount, scooterDtos);
        }

        public async Task<ScooterDto> GetScooterByIdAsync(Guid id)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetEntityWithSpecAsync(new ScooterByIdSpecification(id));

            if (scooter is null)
                throw new NotFoundException("Scooter", id);

            return scooter.ToDto();
        }

        public async Task<ScooterStatusDto> GetScooterStatusAsync(string serialNumber)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetEntityWithSpecAsync(new ScooterBySerialNumberSpecification(serialNumber));

            if (scooter is null)
                throw new NotFoundException("Scooter", serialNumber);

            return scooter.ToStatusDto();
        }

        public async Task<ScooterDto> CreateScooterAsync(ScooterForCreationDto scooterDto)
        {
            var scooter = scooterDto.ToEntity();

            _unitOfWork.GetRepository<Scooter>().Add(scooter);

            await _unitOfWork.SaveChangesAsync();

            return scooter.ToDto();
        }

        public async Task<ScooterDto> UpdateScooterAsync(Guid id, ScooterForUpdateDto scooterDto)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetEntityWithSpecAsync(new ScooterByIdSpecification(id));

            if (scooter is null)
                throw new NotFoundException("Scooter", id);

            scooterDto.UpdateEntity(scooter);

            _unitOfWork.GetRepository<Scooter>().Update(scooter);

            await _unitOfWork.SaveChangesAsync();

            var updatedScooterDto = scooter.ToDto();

            return updatedScooterDto;
        }

        public async Task<bool> DeleteScooterAsync(Guid id)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(id);

            if (scooter is null)
                throw new NotFoundException("Scooter", id);

            _unitOfWork.GetRepository<Scooter>().Delete(scooter);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
