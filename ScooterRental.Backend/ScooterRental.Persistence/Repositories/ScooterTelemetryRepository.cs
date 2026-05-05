namespace ScooterRental.Persistence.Repositories
{
    public class ScooterTelemetryRepository(IConnectionMultiplexer _redisConnection) : IScooterTelemetryRepository
    {
        private readonly IDatabase _database = _redisConnection.GetDatabase();

        public async Task<ScooterTelemetry?> SaveOrUpdateTelemetryAsync(ScooterTelemetry telemetry, TimeSpan? timeToLive = null)
        {
            string key = $"scooter:telemetry:{telemetry.SerialNumber}";

            var value = JsonSerializer.Serialize(telemetry);

            var result = await _database.StringSetAsync(key, value, timeToLive ?? TimeSpan.FromMinutes(15));

            if (!result)
                return null;

            return telemetry;
        }

        public async Task<IEnumerable<ScooterTelemetry>> GetAllActiveTelemetriesAsync()
        {
            var endpoint = _redisConnection.GetEndPoints().First();

            var server = _redisConnection.GetServer(endpoint);

            var keys = server.Keys(pattern: "scooter:telemetry:*").ToArray();

            var jsonTelemetries = await _database.StringGetAsync(keys);

            var activeScooters = new List<ScooterTelemetry>(jsonTelemetries.Length);

            foreach (var jsonTelemetry in jsonTelemetries)
            {
                if (jsonTelemetry.IsNullOrEmpty)
                    continue;

                var telemetry = JsonSerializer.Deserialize<ScooterTelemetry>(jsonTelemetry);

                if (telemetry != null)
                {
                    activeScooters.Add(telemetry);
                }
            }
            return activeScooters;
        }
        
        public async Task<ScooterTelemetry?> GetLatestTelemetryAsync(string SerialNumber)
        {
            string key = $"scooter:telemetry:{SerialNumber}";

            var telemetry = await _database.StringGetAsync(key);

            if (telemetry.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<ScooterTelemetry>(telemetry);
        }

        public async Task<bool> RemoveTelemetryAsync(string SerialNumber)
        {
            string key = $"scooter:telemetry:{SerialNumber}";

            return await _database.KeyDeleteAsync(key);
        }
    }
}
