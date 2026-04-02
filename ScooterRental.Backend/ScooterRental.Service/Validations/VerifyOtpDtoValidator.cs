namespace ScooterRental.Service.Validations
{
    public class VerifyOtpDtoValidator : AbstractValidator<VerifyOtpDto>
    {
        public VerifyOtpDtoValidator()
        {            
            RuleFor(x => x.Email).EmailAddress()
                .NotEmpty().WithMessage("A valid email address is required.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be exactly 6 digits.")
                .Matches("^[0-9]+$").WithMessage("OTP must contain only numbers.");
        }
    }
}
