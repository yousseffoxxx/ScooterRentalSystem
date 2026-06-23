namespace ScooterRental.Domain.Exceptions
{
    public sealed class UnAuthorizedException(string message) : AppException(message, HttpStatusCode.Unauthorized)
    {
    }
}
