using KingTech.P1Reader.Message;
using Microsoft.Extensions.Logging;

namespace KingTech.P1Reader;

/// <inheritdoc/>
public class BasicP1Receiver : ABaseP1Receiver
{
    /// <summary>
    /// Event that is triggered whenever a new telegram is received via the P1 connection.
    /// </summary>
    public delegate Task TelegramReceivedEvent(P1Message telegram);
    /// <summary>
    /// Event that is triggered whenever a new telegram is received via the P1 connection.
    /// </summary>
    public TelegramReceivedEvent OnTelegramReceived { get; set; }

    /// <summary>
    /// The P1 receiver is responsible for receiving and parsing P1 messages using the given configuration.
    /// </summary>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create loggers for the P1Receiver system.</param>
    /// <param name="settings">The <see cref="P1ReceiverSettings"/> for this receiver.</param>
    public BasicP1Receiver(ILoggerFactory? loggerFactory, P1ReceiverSettings settings) : base(loggerFactory, settings)
    {
        
    }

    /// <inheritdoc/>
    protected override void HandleP1Message(P1Message message)
    {
        OnTelegramReceived?.Invoke(message).Wait();
    }
}