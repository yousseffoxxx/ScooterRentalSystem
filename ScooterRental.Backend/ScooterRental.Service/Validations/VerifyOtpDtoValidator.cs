namespace ScooterRental.Service.Validations
{
    public class VerifyOtpDtoValidator : AbstractValidator<VerifyOtpDto>
    {
        public VerifyOtpDtoValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^01[0125][0-9]{8}$").WithMessage("Phone number must be a valid Egyptian mobile number (e.g., 01012345678).");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be exactly 6 digits.")
                .Matches("^[0-9]+$").WithMessage("OTP must contain only numbers.");
        }
    }
}
