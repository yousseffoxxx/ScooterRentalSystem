namespace ScooterRental.Service.ScooterServices
{
    public class ScooterService(IUnitOfWork _unitOfWork, IMqttCommandService _mqttCommandService) : IScooterService
    {
        public async Task<PaginatedResult<ScooterDto>> GetAllScootersAsync(QueryParams queryParams)
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

        public async Task<ScooterCreatedResultDto> CreateScooterAsync(ScooterForCreationDto scooterDto)
        {
            var scooterInDb = await _unitOfWork.GetRepository<Scooter>().GetEntityWithSpecAsync(new ScooterBySerialNumberSpecification(scooterDto.SerialNumber));

            if (scooterInDb is not null)
                throw new BadRequestException($"Scooter with Serial number {scooterDto.SerialNumber} already exists in the DB");
            
            var deviceSecretKey = IotSecurityHelper.GenerateDeviceSecret();
            
            var scooter = scooterDto.ToEntity(deviceSecretKey);

            _unitOfWork.GetRepository<Scooter>().Add(scooter);

            await _unitOfWork.SaveChangesAsync();

            var standardDto = scooter.ToDto();

            return new ScooterCreatedResultDto(standardDto, deviceSecretKey);
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

        public async Task<LiveMapDto> GetLiveMapDataAsync()
        {
            var zones = await _unitOfWork.GetRepository<Zone>().GetAllWithSpecAsync(new AllZonesSpecification(true));
            
            var zonesMapDtos = zones.ToMapDtoList();

            var tariff = await _unitOfWork.GetRepository<Tariff>().GetEntityWithSpecAsync(new GetActiveTariffSpec());
            
            if (tariff is null)
                throw new NotFoundException("Tariff", 0);

            var scooters = await _unitOfWork.GetRepository<Scooter>().GetAllWithSpecAsync(new AllScootersSpecification());

            var scootersMapDtos = scooters.ToMapDtoList(tariff.UnlockFee,tariff.PerMinuteRate);

            return new LiveMapDto(scootersMapDtos, zonesMapDtos);
        }

        public async Task<bool> ForceUnlockScooterAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);
            
            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            await _mqttCommandService.SendCommandAsync(scooter.SerialNumber, ScooterCommandType.UnlockScooter);

            return true;
        }

        public async Task<bool> ForceLockScooterAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);
            
            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            await _mqttCommandService.SendCommandAsync(scooter.SerialNumber, ScooterCommandType.LockScooter, 0);

            return true;
        }

        public async Task<bool> PlayScooterAlarmAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);
            
            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            await _mqttCommandService.SendCommandAsync(scooter.SerialNumber, ScooterCommandType.PlayAlarm);

            return true;
        }

        public async Task<bool> PutScooterInMaintenanceAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);

            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            if (scooter.Status == ScooterStatus.InUse)
                throw new BadRequestException("Cannot put a scooter into maintenance while it is in an active ride.");

            scooter.Status = ScooterStatus.Maintenance;

            _unitOfWork.GetRepository<Scooter>().Update(scooter);
            
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RetireScooterAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);

            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            if (scooter.Status == ScooterStatus.InUse)
                throw new BadRequestException("Cannot retire a scooter while it is in an active ride.");

            scooter.Status = ScooterStatus.Offline;

            _unitOfWork.GetRepository<Scooter>().Update(scooter);
            
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForceStartScooterAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);

            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            await _mqttCommandService.SendCommandAsync(scooter.SerialNumber, ScooterCommandType.StartScooter);

            return true;
        }

        public async Task<bool> ForceStopScooterAsync(Guid scooterId)
        {
            var scooter = await _unitOfWork.GetRepository<Scooter>().GetByIdAsync(scooterId);

            if (scooter is null)
                throw new NotFoundException("Scooter", scooterId);

            await _mqttCommandService.SendCommandAsync(scooter.SerialNumber, ScooterCommandType.StopScooter, 0);

            return true;
        }
    }
}
