namespace ScooterRental.Shared.DTOs.Zone.Response
{
    public record ZoneDto(Guid Id, string Name, string Type, double? SpeedLimitKmH, bool IsActive, IEnumerable<CoordinateDto> Boundary)
    {
    }
}
