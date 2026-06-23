namespace ScooterRental.Persistence.Repositories
{
    public class ActiveRideCacheRepository(IConnectionMultiplexer _redisConnection) : IActiveRideCacheRepository
    {
        private readonly IDatabase _database = _redisConnection.GetDatabase();

        public async Task<ActiveRideCacheModel?> GetActiveRideAsync(string serialNumber)
        {
            var key = $"active_ride:{serialNumber}";

            var json = await _database.StringGetAsync(key);

            if (json.IsNullOrEmpty) 
                return null;

            return JsonSerializer.Deserialize<ActiveRideCacheModel>(json.ToString());
        }

        public async Task<bool> RemoveActiveRideAsync(string serialNumber)
        {
            var key = $"active_ride:{serialNumber}";

            return await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> SetActiveRideAsync(ActiveRideCacheModel ride)
        {
            var key = $"active_ride:{ride.SerialNumber}";

            var json = JsonSerializer.Serialize(ride);

            return await _database.StringSetAsync(key, json, TimeSpan.FromHours(24));
        }
    }
}
