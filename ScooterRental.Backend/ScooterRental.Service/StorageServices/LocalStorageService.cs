namespace ScooterRental.Service.StorageServices
{
    public class LocalStorageService(IWebHostEnvironment _environment) : ILocalStorageService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file is null || file.Length == 0)
                throw new BadRequestException("Id Photo is Empty");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var folderPath = Path.Combine(_environment.WebRootPath, folderName);

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
