namespace ScooterRental.Service.Validations.Tariff
{
    public class TariffForCreationDtoValidator : AbstractValidator<TariffForCreationDto>
    {
        public TariffForCreationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");
            
            RuleFor(x => x.UnlockFee)
            .GreaterThanOrEqualTo(0).WithMessage("Unlock fee cannot be negative.");

            RuleFor(x => x.PerMinuteRate)
                .GreaterThan(0).WithMessage("Per minute rate must be greater than zero.");
        }
    }
}
