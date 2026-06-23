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
                .WithTcpServer(_options.Value.BrokerAddress, _options.Value.Port)
                .WithClientId(_options.Value.ClientId)
                .WithCredentials(_options.Value.Username, _options.Value.Password)
                .WithTlsOptions(o => o.UseTls())
                .WithCleanSession()
                .Build();

            _logger.LogInformation("Connecting to MQTT Broker...");

            mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessage;

            await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

            _logger.LogInformation("Connected! Subscribing to topic...");

            var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(_options.Value.Topic)).Build();

            await mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);

            mqttClient.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("Disconnected from MQTT Broker! Attempting to reconnect...");

                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(mqttClientOptions, stoppingToken);

                    await mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);

                    _logger.LogInformation("Successfully reconnected and resubscribed!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reconnect to MQTT Broker.");
                }
            };

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var payload = e.ApplicationMessage.ConvertPayloadToString();

                _logger.LogInformation("Received MQTT payload: {Payload}", payload);

                await using var scope = _serviceScopeFactory.CreateAsyncScope();

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
