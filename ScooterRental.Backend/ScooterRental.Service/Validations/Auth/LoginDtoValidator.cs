namespace ScooterRental.Service.Validations.Auth
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^(\+20)?01[0125][0-9]{8}$").WithMessage("Phone number must be valid (e.g., +201012345678 or 01012345678).");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
