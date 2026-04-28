namespace ScooterRental.Shared.DTOs.Scooter.Response
{
    public record ScooterStatusDto(string SerialNumber, int BatteryLevel, string Status)
    {
    }
}
