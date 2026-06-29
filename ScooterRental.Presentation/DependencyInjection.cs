namespace ScooterRental.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddScoped<IRealTimeBroadcastService, SignalRBroadcastService>();

            return services;
        }
    }
}
