namespace ScooterRental.Domain.Exceptions
{
    public sealed class AppValidationException(IDictionary<string, string[]> errors) : AppException("One or more validation errors occurred.", HttpStatusCode.BadRequest)
    {
        public IDictionary<string, string[]> Errors { get; } = errors;
    }
}
