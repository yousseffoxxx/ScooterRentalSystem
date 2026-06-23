namespace ScooterRental.Shared
{
    public record ScooterCommandPayload(ScooterCommandType Command,int? TargetSpeed, DateTimeOffset Timestamp)
    {
    }
}
