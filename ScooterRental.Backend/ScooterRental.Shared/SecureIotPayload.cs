namespace ScooterRental.Shared
{
    public record SecureIotPayload<TEntity>(string SerialNumber, long Timestamp, string Signature, TEntity Data)
    {
    }
}
