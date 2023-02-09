namespace KingTech.P1Reader.Message;

public class FailureEvent
{
    public DateTime? Timestamp { get; set; }
    public long? RawTimestamp { get; set; }
    public int? DurationInSeconds { get; set; }
}