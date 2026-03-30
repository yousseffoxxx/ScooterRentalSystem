namespace ScooterRental.Service.Validations
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).EmailAddress()
                    .NotEmpty().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.");
        }
    }
}
