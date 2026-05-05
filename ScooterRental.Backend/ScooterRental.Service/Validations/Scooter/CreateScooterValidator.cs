using ScooterRental.Shared.DTOs.Scooter.Request;

namespace ScooterRental.Service.Validations.Scooter
{
    public class CreateScooterValidator : AbstractValidator<ScooterForCreationDto>
    {
        public CreateScooterValidator()
        {

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required")
                .MaximumLength(50).WithMessage("Serial number cannot exceed 50 characters.");

        }
    }
}
