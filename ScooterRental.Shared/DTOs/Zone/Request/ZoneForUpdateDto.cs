namespace ScooterRental.Shared.DTOs.Zone.Request
{
    public record ZoneForUpdateDto(string Name, string Type, double? SpeedLimitKmH, bool IsActive, IEnumerable<CoordinateDto> Boundary)
    {
    }
}
