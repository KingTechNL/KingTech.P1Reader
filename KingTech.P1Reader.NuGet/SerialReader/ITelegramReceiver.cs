namespace KingTech.P1Reader.SerialReader;

/// <summary>
/// Reader to consume telegram messages in the P1 format.
/// </summary>
internal interface ITelegramReceiver
{
    /// <summary>
    /// Start reading incoming messages.
    /// </summary>
    /// <returns>True on success, false otherwise.</returns>
    bool Start();
    /// <summary>
    /// Stop reading incoming messages.
    /// </summary>
    /// <returns>True on success, false otherwise.</returns>
    bool Stop();
}