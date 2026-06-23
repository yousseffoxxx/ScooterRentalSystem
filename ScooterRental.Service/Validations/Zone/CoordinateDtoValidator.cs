namespace ScooterRental.Service.Validations.Zone
{
    public class CoordinateDtoValidator : AbstractValidator<CoordinateDto>
    {
        public CoordinateDtoValidator()
        {
            RuleFor(x => x.Longitude)
                        .InclusiveBetween(-180, 180)
                        .WithMessage("Longitude must be between -180 and 180 degrees.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees.");
        }
    }
}
