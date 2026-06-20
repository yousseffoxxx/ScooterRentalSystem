namespace ScooterRental.Shared.DTOs.Auth.Request
{
    public record ResetPasswordDto(string PhoneNumber, string FirebaseToken, string NewPassword)
    {

    }
}
