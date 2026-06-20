namespace ScooterRental.Shared.DTOs.Scooter.Response
{
    public record ScooterStatusDto(string SerialNumber,Guid Id, int BatteryLevel, string Status)
    {
    }
}
