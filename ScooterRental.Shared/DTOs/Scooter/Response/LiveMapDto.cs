namespace ScooterRental.Shared.DTOs.Scooter.Response
{
    public record LiveMapDto(IEnumerable<MapScooterDto> Scooters, IEnumerable<MapZoneDto> Zones)
    {
    }
}
