using KingTech.P1Reader.Message;
using KingTech.P1Reader.Parser;
using KingTech.P1Reader.SerialReader;
using Microsoft.Extensions.Logging;

namespace KingTech.P1Reader;

/// <inheritdoc/>
public abstract class ABaseP1Receiver : IP1Receiver
{
    /// <inheritdoc/>
    public P1Message? LastTelegram { get; private set; }

    protected readonly ILogger<ABaseP1Receiver>? Logger;
    private readonly IP1Parser _parser;

    private readonly ITelegramReceiver _telegramReceiver;

    /// <summary>
    /// The P1 receiver is responsible for receiving and parsing P1 messages using the given configuration.
    /// </summary>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create loggers for the P1Receiver system.</param>
    /// <param name="settings">The <see cref="P1ReceiverSettings"/> for this receiver.</param>
    protected ABaseP1Receiver(ILoggerFactory? loggerFactory, P1ReceiverSettings settings)
    {
        Logger = loggerFactory?.CreateLogger<ABaseP1Receiver>();

        _parser = GetParser(settings.Format);
        _telegramReceiver = settings.SerialPort.ToUpper() == "DUMMY"
            ? new SerialReader.DummyTelegramReceiver(loggerFactory?.CreateLogger<DummyTelegramReceiver>(), HandleTelegram)
            : new SerialReader.SerialTelegramReceiver(loggerFactory?.CreateLogger<SerialReader.SerialTelegramReceiver>(), settings.SerialPort, HandleTelegram);
    }

    /// <inheritdoc/>
    public virtual void Start()
    {
        _telegramReceiver.Start();
    }

    /// <inheritdoc/>
    public virtual void Stop()
    {
        _telegramReceiver.Stop();
    }

    /// <summary>
    /// Handle an incoming telegram message.
    /// </summary>
    /// <param name="telegramMessage">The telegram message to handle.</param>
    protected virtual void HandleTelegram(string telegramMessage)
    {
        Logger?.LogDebug("Received new message: {message}", telegramMessage);

        //Parse the message.
        var message = _parser.ParseTelegram(telegramMessage);
        if (message == null)
            return;

        HandleP1Message(message);

        //Update values.
        LastTelegram = message;
    }

    /// <summary>
    /// Handle the incoming P1 message.
    /// </summary>
    /// <param name="message">The received and parsed P1 message.</param>
    protected abstract void HandleP1Message(P1Message message);

    /// <summary>
    /// Get the telegram parser for the given format name.
    /// </summary>
    /// <param name="format">The format name.</param>
    /// <returns>The telegram parser for the given name.</returns>
    private IP1Parser GetParser(string format)
    {
        switch (format.ToUpper())
        {
            case "XS210ESMR5":
                return new XS210ESMR5Parser();
            case "V502":
                return new V502Parser();
            default:
                Logger?.LogError("Unknown format: {format}. Using default format: v502", format);
                return new V502Parser();
        }
    }
}