namespace ScooterRental.Service.Validations
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

            RuleFor(x => x.AvatarUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.AvatarUrl))
                .WithMessage("ID Photo must be a valid URL.");

        }
    }
}
