namespace ScooterRental.Service.AuthServices
{
    public class AiVerificationService(HttpClient _httpClient, ILogger<AiVerificationService> _logger) : IAiVerificationService
    {
        public async Task<AiVerificationResponseDto> VerifyIdentityAsync(IFormFile idFront, IFormFile idBack, IFormFile selfie)
        {
            using var formData = new MultipartFormDataContent();

            // ID Front
            var idFrontStream = new StreamContent(idFront.OpenReadStream());
            
            idFrontStream.Headers.ContentType = new MediaTypeHeaderValue(idFront.ContentType);

            formData.Add(idFrontStream, "id_image", idFront.FileName);

            // ID Back
            var idBackStream = new StreamContent(idBack.OpenReadStream());

            idBackStream.Headers.ContentType = new MediaTypeHeaderValue(idBack.ContentType);

            formData.Add(idBackStream, "id_back", idBack.FileName);

            // Selfie
            var selfieStream = new StreamContent(selfie.OpenReadStream());

            selfieStream.Headers.ContentType = new MediaTypeHeaderValue(selfie.ContentType);

            formData.Add(selfieStream, "selfie", selfie.FileName);

            // send request
            var response = await _httpClient.PostAsync("/verify", formData);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(response.Content.ToString());

                throw new BadRequestException("The AI verification service is currently unavailable. Please try again later.");
            }

            var result = await response.Content.ReadFromJsonAsync<AiVerificationResponseDto>();

            return result?? throw new BadRequestException("Failed to deserialize AI verification response.");
        }
    }
}
