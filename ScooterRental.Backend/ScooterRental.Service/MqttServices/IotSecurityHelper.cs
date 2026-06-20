namespace ScooterRental.Service.MqttServices
{
    public static class IotSecurityHelper
    {
        public static string GenerateDeviceSecret()
        {
            var keyBytes = new byte[32];

            RandomNumberGenerator.Fill(keyBytes);

            return Convert.ToBase64String(keyBytes);
        }
    }
}
