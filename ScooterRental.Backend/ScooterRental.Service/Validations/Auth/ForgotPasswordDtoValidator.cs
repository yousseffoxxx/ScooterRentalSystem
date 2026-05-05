namespace ScooterRental.Service.Validations.Auth
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("A valid email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.");
        }
    }
}
