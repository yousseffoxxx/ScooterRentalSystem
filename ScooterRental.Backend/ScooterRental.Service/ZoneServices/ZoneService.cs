namespace ScooterRental.Service.ZoneServices
{
    public class ZoneService(IUnitOfWork _unitOfWork,IValidator<ZoneForCreationDto> _createValidator,
        IValidator<ZoneForUpdateDto> _updateValidator, IRedisZoneEventPublisher _redisZoneEventPublisher) : IZoneService
    {
        public async Task<PaginatedResult<ZoneDto>> GetAllZonesAsync(ZoneQueryParams queryParams)
        {
            var spec = new AllZonesSpecification(queryParams.PageIndex, queryParams.PageSize,queryParams.IsActive);

            var repo = _unitOfWork.GetRepository<Zone>();

            var zones = await repo.GetAllWithSpecAsync(spec);

            var totalCount = await repo.CountAsync(spec);

            var dtoList = zones.ToDtoList();

            return new PaginatedResult<ZoneDto>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtoList);
        }

        public async Task<ZoneDto> GetZoneByIdAsync(Guid id)
        {
            var spec = new ZoneByIdSpecification(id);

            var repo = _unitOfWork.GetRepository<Zone>();

            var zone = await repo.GetEntityWithSpecAsync(spec);

            if (zone is null)
                throw new NotFoundException("Zone", id);

            var dto = zone.ToDto();

            return dto;
        }

        public async Task<IReadOnlyList<ZoneDto>> GetZoneByLocationAsync(double longitude, double latitude)
        {
            var targetPoint = new Point(longitude, latitude) { SRID = 4326 };

            var spec = new ZoneByLocationSpecification(targetPoint);

            var repo = _unitOfWork.GetRepository<Zone>();

            var zones = await repo.GetAllWithSpecAsync(spec);

            var dtos = zones.ToDtoList();

            return dtos;
        }

        public async Task<ZoneDto> CreateZoneAsync(ZoneForCreationDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                throw new ValidationException("",validationResult.Errors);

            var zone = dto.ToEntity();

            _unitOfWork.GetRepository<Zone>().Add(zone);

            await _unitOfWork.SaveChangesAsync();

            var zoneCacheEvent = new ZoneCacheInvalidatedEvent(zone.Id, "Created");

            await _redisZoneEventPublisher.PublishZoneCacheInvalidationAsync(zoneCacheEvent);

            var zoneDto = zone.ToDto();

            return zoneDto;       
        }

        public async Task<ZoneDto> UpdateZoneAsync(ZoneForUpdateDto dto, Guid id)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                throw new ValidationException("", validationResult.Errors);

            var repo = _unitOfWork.GetRepository<Zone>();

            var zone = await repo.GetByIdAsync(id);

            if (zone is null)
                throw new NotFoundException("Zone", id);

            dto.UpdateEntity(zone);

            repo.Update(zone);

            await _unitOfWork.SaveChangesAsync();

            var zoneCacheEvent = new ZoneCacheInvalidatedEvent(zone.Id, "Updated");

            await _redisZoneEventPublisher.PublishZoneCacheInvalidationAsync(zoneCacheEvent);

            var updatedDto = zone.ToDto();

            return updatedDto;
        }

        public async Task<bool> DeleteZoneAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Zone>();

            var zone = await repo.GetByIdAsync(id);

            if (zone is null)
                throw new NotFoundException("Zone", id);

            repo.Delete(zone);

            await _unitOfWork.SaveChangesAsync();

            var zoneCacheEvent = new ZoneCacheInvalidatedEvent(zone.Id, "Deleted");

            await _redisZoneEventPublisher.PublishZoneCacheInvalidationAsync(zoneCacheEvent);

            return true;
        }
    }
}
