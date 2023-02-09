namespace KingTech.P1Reader.Message;

public class MBusClient
{
    public string? DeviceIdentifier { get; set; } //0-n:96.1.0
    public long? DeviceType { get; set; } //0-n:24.1.0
    public DateTime? CaptureTime { get; set; } //0-1:24.2.1(101209112500W)(12785.123* m3)
    public long? RawCaptureTime { get; set; } //0-1:24.2.1(101209112500W)(12785.123* m3)

    public double? Value { get; set; } //0-1:24.2.1(101209112500W)(12785.123* m3)
}