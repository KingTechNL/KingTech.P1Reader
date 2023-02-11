namespace KingTech.P1Reader.Models;

/// <summary>
/// Model for the data that is received from the ModbusClient.
/// This is usually a gas or water meter.
/// </summary>
public class ModbusClientModel
{
    /// <summary>
    /// The unique identifier of the device.
    /// P1 key: 0-n:96.1.0
    /// </summary>
    public string? DeviceIdentifier { get; set; }
    /// <summary>
    /// The type of the device.
    /// P1 key: 0-n:24.1.0
    /// </summary>
    public long? DeviceType { get; set; }
    /// <summary>
    /// The timestamp of the last measurement as DateTime.
    /// P1 example: 0-n:24.2.1(101209112500W)(12785.123* m3)
    /// </summary>
    public DateTime? CaptureTime { get; set; }
    /// <summary>
    /// The timestamp of the last measurement as it was received.
    /// P1 example: 0-n:24.2.1(101209112500W)(12785.123* m3)
    /// </summary>
    public long? RawCaptureTime { get; set; }
    /// <summary>
    /// The value of the last measurement.
    /// P1 example: 0-n:24.2.1(101209112500W)(12785.123* m3)
    /// </summary>
    public double? Value { get; set; }
}