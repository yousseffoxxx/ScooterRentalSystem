namespace ScooterRental.Service.Abstractions.ScooterServices
{
    public interface IScooterService
    {
        // Mobile / Shared
        Task<ScooterStatusDto> GetScooterStatusAsync(string serialNumber);
        Task<LiveMapDto> GetLiveMapDataAsync();

        // Admin
        Task<PaginatedResult<ScooterDto>> GetAllScootersAsync(QueryParams queryParams);
        Task<ScooterDto> GetScooterByIdAsync(Guid id);
        Task<ScooterDto> CreateScooterAsync(ScooterForCreationDto scooterDto);
        Task<ScooterDto> UpdateScooterAsync(Guid id, ScooterForUpdateDto scooterDto);
        Task<bool> DeleteScooterAsync(Guid id);
        Task<bool> ForceUnlockScooterAsync(Guid scooterId);
        Task<bool> ForceLockScooterAsync(Guid scooterId);
        Task<bool> PlayScooterAlarmAsync(Guid scooterId);
        Task<bool> PutScooterInMaintenanceAsync(Guid scooterId);
        Task<bool> RetireScooterAsync(Guid scooterId);
    }
}
