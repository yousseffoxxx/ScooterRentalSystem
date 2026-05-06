namespace ScooterRental.Service.Validations.Auth
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {

            RuleFor(r => r.FullName)
                .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            RuleFor(r => r.PhoneNumber)
                .Matches(@"^01[0125][0-9]{8}$").WithMessage("Phone number must be a valid Egyptian mobile number (e.g., 01012345678).");

            RuleFor(x => x.AvatarPhoto)
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
