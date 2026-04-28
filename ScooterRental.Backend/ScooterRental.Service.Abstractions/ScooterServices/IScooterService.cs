using ScooterRental.Shared.DTOs.Scooter.Response;

namespace ScooterRental.Service.Abstractions.ScooterServices
{
    public interface IScooterService
    {
        // Mobile / Shared
        Task<ScooterStatusDto> GetScooterStatusAsync(string serialNumber);

        // Admin
        Task<PaginatedResult<ScooterDto>> GetAllScootersAsync(ScooterQueryParams queryParams);
        Task<ScooterDto> GetScooterByIdAsync(Guid id);
        Task<ScooterDto> CreateScooterAsync(ScooterForCreationDto scooterDto);
        Task<ScooterDto> UpdateScooterAsync(Guid id, ScooterForUpdateDto scooterDto);
        Task<bool> DeleteScooterAsync(Guid id);
    }
}
