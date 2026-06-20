namespace ScooterRental.Service.Validations.AdminManagement.Auth
{
    public class LoginAdminDtoValidator : AbstractValidator<LoginAdminDto>
    {
        public LoginAdminDtoValidator()
        {
            RuleFor(x => x.Email).EmailAddress()
                .NotEmpty().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}
