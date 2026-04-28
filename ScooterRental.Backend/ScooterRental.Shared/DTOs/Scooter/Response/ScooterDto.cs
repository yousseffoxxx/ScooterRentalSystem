namespace ScooterRental.Shared.DTOs.Scooter.Response
{
    public record ScooterDto(Guid Id, string SerialNumber, int BatteryLevel,string Status, string ModelName)
    {
    }
}
