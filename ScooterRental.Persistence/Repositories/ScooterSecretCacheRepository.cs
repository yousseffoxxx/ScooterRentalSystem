namespace ScooterRental.Persistence.Repositories
{
    public class ScooterSecretCacheRepository(IConnectionMultiplexer _redisConnection) : IScooterSecretCacheRepository
    {
        private readonly IDatabase _database = _redisConnection.GetDatabase();

        public async Task<string> GetSecretAsync(string serialNumber)
        {
            var key = $"secret_key:{serialNumber}";

            var json = await _database.StringGetAsync(key);

            if (json.IsNullOrEmpty)
                return null;

            return json.ToString();
        }

        public async Task<bool> SetSecretAsync(string serialNumber, string secretKey)
        {
            var key = $"secret_key:{serialNumber}";

            return await _database.StringSetAsync(key, secretKey, TimeSpan.FromDays(7));
        }
    }
}
