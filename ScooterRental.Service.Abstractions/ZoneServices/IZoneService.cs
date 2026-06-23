namespace ScooterRental.Service.Abstractions.ZoneServices
{
    public interface IZoneService
    {
        Task<ZoneDto> CreateZoneAsync(ZoneForCreationDto dto);
        Task<PaginatedResult<ZoneDto>> GetAllZonesAsync(ZoneQueryParams queryParams);
        Task<ZoneDto> GetZoneByIdAsync(Guid id);
        Task<ZoneDto> UpdateZoneAsync(ZoneForUpdateDto dto, Guid id);
        Task<bool> DeleteZoneAsync(Guid id);
        Task<IReadOnlyList<ZoneDto>> GetZoneByLocationAsync(double longitude, double latitude);
    }
}
