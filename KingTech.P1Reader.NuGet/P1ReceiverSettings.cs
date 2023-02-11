namespace KingTech.P1Reader;

/// <summary>
/// General settings for the P1 receiver.
/// </summary>
public class P1ReceiverSettings
{
    /// <summary>
    /// The serial port to listen on.
    /// </summary>
    /// <example>COM1</example>
    /// <example>/dev/ttyUSB0</example>
    public string SerialPort { get; set; } = "COM1";

    /// <summary>
    /// The format of incoming P1 messages.
    /// Used to decide which parser to use.
    /// </summary>
    /// <example>V502</example>
    public string Format { get; set; } = "V502";
}