using Microsoft.Extensions.Hosting;

namespace ScooterRental.Service.StorageServices
{
    public class LocalStorageService(IHostEnvironment _environment) : ILocalStorageService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file is null || file.Length == 0)
                throw new BadRequestException("File is Empty");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            var folderPath = Path.Combine(wwwrootPath, folderName);

            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"{folderName}/{fileName}";
        }
    }
}
