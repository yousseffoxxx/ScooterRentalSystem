namespace ScooterRental.MqttWorker
{
    public class MqttTelemetryWorker(ILogger<MqttTelemetryWorker> _logger,
        IOptions<MqttOptions> _options, IServiceScopeFactory _serviceScopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttFactory = new MqttClientFactory();

            var mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_options.Value.BrokerAddress, _options.Value.Port).WithClientId(_options.Value.ClientId).Build();

            _logger.LogInformation("Connecting to MQTT Broker...");

            mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessage;

            await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

            _logger.LogInformation("Connected! Subscribing to topic...");

            var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(_options.Value.Topic)).Build();

            await mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var payload = e.ApplicationMessage.ConvertPayloadToString();

                _logger.LogInformation("Received MQTT payload: {Payload}", payload);

                using var scope = _serviceScopeFactory.CreateAsyncScope();

                var service = scope.ServiceProvider.GetRequiredService<IScooterTelemetryService>();

                await service.ProcessIncomingTelemetryAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A fatal error occurred while processing MQTT message.");
            }
        }
    }
}
