namespace ScooterRental.Shared.DTOs.Zone.Request
{
    public record ZoneForCreationDto(string Name, string Type, double? SpeedLimitKmH, IEnumerable<CoordinateDto> Boundary)
    {
    }
}
