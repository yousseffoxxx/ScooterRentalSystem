namespace ScooterRental.MqttService
{
    public class MqttCommandService(ILogger<MqttCommandService> _logger,
        IOptions<MqttOptions> _options) : IMqttCommandService
    {
        public async Task SendCommandAsync(string serialNumber, ScooterCommandType command, int value)
        {
            try
            {
                var mqttFactory = new MqttClientFactory();

                using var mqttClient = mqttFactory.CreateMqttClient();

                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_options.Value.BrokerAddress, _options.Value.Port).Build();

                _logger.LogInformation("Connecting to MQTT Broker...");

                await mqttClient.ConnectAsync(mqttClientOptions);

                var payloadRecord = new ScooterCommandPayload(command, value);

                var payload = JsonSerializer.Serialize(payloadRecord);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"scooterrental/commands/{serialNumber}").WithPayload(payload).Build();

                await mqttClient.PublishAsync(message);

                _logger.LogInformation("Sent {Command} command to scooter {Serial}", command, serialNumber);

                await mqttClient.DisconnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical failure while sending {Command} command to scooter {Serial}", command, serialNumber);
            }
        }
    }
}
