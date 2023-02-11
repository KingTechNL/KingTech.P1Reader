using KingTech.P1Reader.Message;

namespace KingTech.P1Reader;

/// <summary>
/// The P1 receiver is responsible for receiving and parsing P1 messages using the given configuration.
/// </summary>
public interface IP1Receiver
{
    /// <summary>
    /// The last telegram message that was received via the P1 connection.
    /// </summary>
    P1Message? LastTelegram { get; }

    /// <summary>
    /// Start handling new P1 messages.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop handling P1 messages.
    /// </summary>
    void Stop();
}