namespace ScooterRental.Shared.DTOs.Ride.Request
{
    public record ReviewParkingPhotoDto(bool IsApproved,string? RejectionReason, decimal PenaltyAmount)
    {
    }
}
