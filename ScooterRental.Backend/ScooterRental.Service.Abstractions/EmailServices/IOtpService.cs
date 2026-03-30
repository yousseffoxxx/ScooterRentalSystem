namespace ScooterRental.Service.Abstractions.AuthServices
{
    public interface IOtpService
    {
        Task<bool> SendOtpAsync(User user);

        Task<bool> VerifyOtpAsync(User user, string code);
    }
}
