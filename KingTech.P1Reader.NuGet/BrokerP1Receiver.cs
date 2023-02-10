using KingTech.P1Reader.Broker;
using KingTech.P1Reader.Message;
using Microsoft.Extensions.Logging;

namespace KingTech.P1Reader;

/// <inheritdoc/>
public class BrokerP1Receiver : ABaseP1Receiver
{
    private readonly IMessageBroker<P1Message> _messageBroker;

    /// <summary>
    /// The P1 receiver is responsible for receiving and parsing P1 messages using the given configuration.
    /// </summary>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create loggers for the P1Receiver system.</param>
    /// <param name="settings">The <see cref="P1ReceiverSettings"/> for this receiver.</param>
    /// <param name="messageBroker">The <see cref="IMessageBroker{TMessage}"/> incoming messages are published on.</param>
    public BrokerP1Receiver(ILoggerFactory? loggerFactory, P1ReceiverSettings settings, 
        IMessageBroker<P1Message> messageBroker) : base(loggerFactory, settings)
    {
        _messageBroker = messageBroker;
    }

    /// <inheritdoc/>
    protected override void HandleP1Message(P1Message message)
    {
        if(!_messageBroker.Enqueue(message))
            Logger?.LogError("Failed to enqueue message {message}", message);
    }
}