using ScooterRental.Shared.DTOs.Scooter.Request;

namespace ScooterRental.Service.Validations.Scooter
{
    public class UpdateScooterValidator : AbstractValidator<ScooterForUpdateDto>
    {
        public UpdateScooterValidator()
        {
            RuleFor(x=>x.Status)
                .IsInEnum().WithMessage("The provided status is invalid. Please provide a valid Scooter Status.");
        }
    }
}
