namespace ScooterRental.MqttService.Abstractions
{
    public interface IMqttCommandService
    {
        Task SendCommandAsync(string serialNumber, ScooterCommandType command, int value);
    }
}
