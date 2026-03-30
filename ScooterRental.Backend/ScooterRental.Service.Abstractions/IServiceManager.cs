namespace ScooterRental.Service.Abstractions
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
    }
}
