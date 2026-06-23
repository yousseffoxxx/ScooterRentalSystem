namespace ScooterRental.Service.Validations.Zone
{
    public class ZoneForUpdateDtoValidator : AbstractValidator<ZoneForUpdateDto>
    {
        public ZoneForUpdateDtoValidator()
        {
            RuleFor(z => z.Name)
                .NotEmpty().WithMessage("Zone name is required.")
                .MaximumLength(100).WithMessage("Zone name cannot exceed 100 characters.");

            RuleFor(z => z.Type)
                .NotEmpty().WithMessage("Zone type is required.")
                .IsEnumName(typeof(ZoneType), false).WithMessage("Invalid zone type. Valid values are: Operational, NoParking.");

            RuleFor(z => z.SpeedLimitKmH)
                .InclusiveBetween(0, 60);

            RuleFor(z => z.Boundary)
                .NotEmpty().WithMessage("A zone boundary must be provided.")
                .Must(b => b != null && b.Count() >= 3).WithMessage("A zone boundary must contain at least 3 coordinates to form a valid polygon.");

            RuleForEach(z => z.Boundary)
                .SetValidator(new CoordinateDtoValidator());
        }
    }
}
