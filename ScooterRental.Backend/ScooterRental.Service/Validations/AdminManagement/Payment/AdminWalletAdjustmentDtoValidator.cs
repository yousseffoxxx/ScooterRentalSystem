namespace ScooterRental.Service.Validations.AdminManagement.Payment
{
    public class AdminWalletAdjustmentDtoValidator : AbstractValidator<AdminWalletAdjustmentDto>
    {
        public AdminWalletAdjustmentDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required to process an adjustment.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Adjustment amount must be greater than zero.")
                .LessThanOrEqualTo(5000).WithMessage("For security reasons, manual adjustments cannot exceed 5000 EGP at a time.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason must be provided for the financial audit log.")
                .MinimumLength(5).WithMessage("Please provide a more descriptive reason (e.g., 'Refund for broken scooter').")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
