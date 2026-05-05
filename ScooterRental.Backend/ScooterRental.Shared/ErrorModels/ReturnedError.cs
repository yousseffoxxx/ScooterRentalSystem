namespace ScooterRental.Shared.ErrorModels
{
    public class ReturnedError
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = null!;
        public IDictionary<string, string[]>? Errors { get; set; }

    }
}
