namespace ScooterRental.Service.Abstractions.RepositoryContracts
{
    public interface IActiveRideCacheRepository
    {
        Task<bool> SetActiveRideAsync(ActiveRideCacheModel ride);
        Task<ActiveRideCacheModel?> GetActiveRideAsync(string serialNumber);
        Task<bool> RemoveActiveRideAsync(string serialNumber);
    }
}
