namespace ScooterRental.Service.Abstractions.StorageServices
{
    public interface ILocalStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
    }
}
