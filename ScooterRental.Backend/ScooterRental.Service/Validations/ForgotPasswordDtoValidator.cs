namespace ScooterRental.Service.Validations
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
