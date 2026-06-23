namespace ScooterRental.Shared
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ScooterCommandType
    {
        StopScooter,
        StartScooter,
        LockScooter,
        UnlockScooter,
        PlayAlarm,
        StopAlarm
    }
}
