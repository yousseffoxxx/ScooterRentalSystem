namespace ScooterRental.Service.Abstractions.AuthServices
{
    public interface IAiVerificationService
    {
        Task<AiVerificationResponseDto> VerifyIdentityAsync(IFormFile idFront, IFormFile idBack, IFormFile selfie);
    }
}
