namespace ScooterRental.Service.Abstractions
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
        IScooterService ScooterService { get; }
        IZoneService ZoneService { get; }
        IRideService RideService { get; }
    }
}
