namespace ScooterRental.Service.Abstractions.RepositoryContracts
{
    public interface IScooterSecretCacheRepository
    {
        Task<string> GetSecretAsync(string serialNumber);
        Task<bool> SetSecretAsync(string serialNumber, string secretKey);
    }
}
