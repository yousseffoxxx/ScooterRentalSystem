namespace ScooterRental.Service.Validations.Ride
{
    public class StartRideValidator : AbstractValidator<StartRideRequestDto>
    {
        public StartRideValidator()
        {
            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required")
                .MaximumLength(50).WithMessage("Serial number cannot exceed 50 characters.");
            
            RuleFor(x => x.UserLongitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180 degrees.");

            RuleFor(x => x.UserLatitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees.");
        }
    }
}
