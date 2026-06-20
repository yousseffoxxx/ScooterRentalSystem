namespace ScooterRental.Service.Validations.Ride
{
    public class EndRideValidator : AbstractValidator<EndRideRequestDto>
    {
        public EndRideValidator()
        {
            RuleFor(x => x.UserLongitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180 degrees.");

            RuleFor(x => x.UserLatitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees.");

            RuleFor(x => x.EndPhoto)
                .NotEmpty().WithMessage("EndPhoto is required.")
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
