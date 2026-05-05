namespace ScooterRental.Service.Abstractions.RepositoryContracts
{
    public interface IScooterTelemetryRepository
    {
        Task<ScooterTelemetry?> GetLatestTelemetryAsync(string SerialNumber);
        Task<IEnumerable<ScooterTelemetry>> GetAllActiveTelemetriesAsync();
        Task<ScooterTelemetry?> SaveOrUpdateTelemetryAsync(ScooterTelemetry telemetry, TimeSpan? timeToLive = null);
        Task<bool> RemoveTelemetryAsync(string SerialNumber);
    }
}
