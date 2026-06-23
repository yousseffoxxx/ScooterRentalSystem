namespace ScooterRental.Shared
{
    public record HardwareTelemetryData
    {
        [JsonPropertyName("batteryLevel")]
        public int BatteryLevel { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("alarm")]
        public bool Alarm { get; set; }
    }
}
