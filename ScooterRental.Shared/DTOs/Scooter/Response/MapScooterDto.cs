namespace ScooterRental.Shared.DTOs.Scooter.Response
{
    public record MapScooterDto(Guid Id, string SerialNumber, int BatteryLevel, double Latitude, double Longitude, decimal UnlockFee, decimal FeePerMinute)
    {
    }
}
