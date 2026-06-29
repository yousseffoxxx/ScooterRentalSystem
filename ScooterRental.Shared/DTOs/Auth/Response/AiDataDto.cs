namespace ScooterRental.Shared.DTOs.Auth.Response
{
    public record AiDataDto
    {
        public string Name { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
    }
}
