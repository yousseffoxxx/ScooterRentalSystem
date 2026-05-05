namespace ScooterRental.Service.Validations.Ride
{
    public class EndRideValidator : AbstractValidator<EndRideRequestDto>
    {
        public EndRideValidator()
        {
            RuleFor(x => x.UserLongitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180 degrees.");

            RuleFor(x => x.UserLatitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees.");

            RuleFor(x => x.EndPhotoUrl)
                .NotEmpty().WithMessage("End Photo URL is required")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("End Photo must be a valid URL.");
        }
    }
}
