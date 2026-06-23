namespace ScooterRental.MqttService.Abstractions
{
    public interface IScooterTelemetryService
    {
        Task ProcessIncomingTelemetryAsync(string jsonPayload);
    }
}
