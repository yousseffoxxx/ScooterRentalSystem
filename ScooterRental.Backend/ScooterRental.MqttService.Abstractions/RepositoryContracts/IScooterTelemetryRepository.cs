namespace ScooterRental.MqttService.Abstractions.RepositoryContracts
{
    public interface IScooterTelemetryRepository
    {
        Task<ScooterTelemetry?> GetTelemetryAsync(Guid id);
        Task<IEnumerable<ScooterTelemetry>> GetAllActiveTelemetriesAsync();
        Task<ScooterTelemetry?> SaveOrUpdateTelemetryAsync(ScooterTelemetry telemetry, TimeSpan? timeToLive = null);
        Task<bool> RemoveTelemetryAsync(Guid id);
    }
}
