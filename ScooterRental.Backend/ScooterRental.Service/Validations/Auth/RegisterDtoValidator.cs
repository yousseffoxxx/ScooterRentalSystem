namespace ScooterRental.Service.Validations.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^01[0125][0-9]{8}$").WithMessage("Phone number must be a valid Egyptian mobile number (e.g., 01012345678).");

            RuleFor(x => x.Email).EmailAddress()
                .NotEmpty().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.IdFrontPhoto)
                .NotEmpty().WithMessage("ID Photo is required.")
                .Must(file => file.Length > 0).WithMessage("The uploaded file is empty.")
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("The file size must not exceed 5 MB.")
                .Must(BeAValidImage).WithMessage("Only JPG and PNG images are allowed.");
            
            RuleFor(x => x.IdBackPhoto)
                .NotEmpty().WithMessage("ID Photo is required.")
                .Must(file => file.Length > 0).WithMessage("The uploaded file is empty.")
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("The file size must not exceed 5 MB.")
                .Must(BeAValidImage).WithMessage("Only JPG and PNG images are allowed.");
        }

        private bool BeAValidImage(IFormFile file)
        {
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png" };

            return allowedContentTypes.Contains(file.ContentType.ToLower());
        }
    }
}
