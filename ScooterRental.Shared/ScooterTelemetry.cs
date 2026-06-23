namespace ScooterRental.Shared
{
    public class ScooterTelemetry
    {
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }


        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("batteryLevel")]
        public int BatteryLevel { get; set; }
        public bool Alarm { get; set; }

        public bool IsOutOfBounds { get; set; }
        public bool IsInNoParkingZone { get; set; }
    }
}
