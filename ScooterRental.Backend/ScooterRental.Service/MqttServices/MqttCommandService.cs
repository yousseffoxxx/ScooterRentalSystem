namespace ScooterRental.Service
{
    public class MqttCommandService(ILogger<MqttCommandService> _logger,
        IOptions<MqttOptions> _options, IScooterSecretCacheRepository _scooterSecretCacheRepository,
        IUnitOfWork _unitOfWork) : IMqttCommandService
    {
        public async Task SendCommandAsync(string serialNumber, ScooterCommandType command, int? targetSpeed = null)
        {
            try
            {
                var mqttFactory = new MqttClientFactory();

                using var mqttClient = mqttFactory.CreateMqttClient();

                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_options.Value.BrokerAddress, _options.Value.Port)
                    .WithClientId(_options.Value.ClientId + "_Publisher_" + Guid.NewGuid().ToString())
                    .WithCredentials(_options.Value.Username, _options.Value.Password)
                    .WithTlsOptions(o => o.UseTls())
                    .Build();

                _logger.LogInformation("Connecting to MQTT Broker...");

                await mqttClient.ConnectAsync(mqttClientOptions);

                var payloadRecord = new ScooterCommandPayload(command, targetSpeed, DateTimeOffset.UtcNow);

                var payloadJson = JsonSerializer.Serialize(payloadRecord);

                var deviceSecretKey = await _scooterSecretCacheRepository.GetSecretAsync(serialNumber);

                if (deviceSecretKey is null)
                {
                    var scooterFromDb = await _unitOfWork.GetRepository<Scooter>().GetEntityWithSpecAsync(new ScooterBySerialNumberSpecification(serialNumber));

                    if (scooterFromDb is null)
                    {
                        _logger.LogWarning("Scooter doesn't exist");
                        return;
                    }

                    deviceSecretKey = scooterFromDb.DeviceSecretKey;

                    await _scooterSecretCacheRepository.SetSecretAsync(scooterFromDb.SerialNumber, scooterFromDb.DeviceSecretKey);
                }

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var keyBytes = Encoding.UTF8.GetBytes(deviceSecretKey);

                var stringToSign = $"{payloadJson}.{timestamp}";

                var payloadBytes = Encoding.UTF8.GetBytes(stringToSign);

                using var hmac = new HMACSHA256(keyBytes);

                var hashBytes = hmac.ComputeHash(payloadBytes);

                var signature = Convert.ToHexString(hashBytes).ToLower();

                var securePayload = new SecureIotPayload<ScooterCommandPayload>(serialNumber, timestamp, signature, payloadRecord);

                var finalJson = JsonSerializer.Serialize(securePayload);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"scooters/{serialNumber}/commands")
                    .WithPayload(finalJson)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

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
