namespace ScooterRental.Shared.DTOs.Zone.Response
{
    public record MapZoneDto(Guid Id, string Name, string Type,IEnumerable<CoordinateDto> Boundary)
    {
    }
}
