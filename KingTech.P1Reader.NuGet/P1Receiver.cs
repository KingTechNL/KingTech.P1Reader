using KingTech.P1Reader.NuGet.Parser;
using Microsoft.Extensions.Logging;

namespace KingTech.P1Reader.NuGet;

/// <inheritdoc/>
public class P1Receiver : IP1Receiver
{
    /// <inheritdoc/>
    public P1Message? LastTelegram { get; private set; }

    /// <inheritdoc/>
    public event EventHandler<P1Message>? TelegramReceived;

    private readonly ILogger<P1Receiver> _logger;
    private readonly IP1Parser _parser;

    private readonly SerialReader.SerialReader _serialReader;

    /// <summary>
    /// The P1 receiver is responsible for receiving and parsing P1 messages using the given configuration.
    /// </summary>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create loggers for the P1Receiver system.</param>
    /// <param name="settings">The <see cref="P1ReceiverSettings"/> for this receiver.</param>
    public P1Receiver(ILoggerFactory loggerFactory, P1ReceiverSettings settings)
    {
        _logger = loggerFactory.CreateLogger<P1Receiver>();

        _parser = GetParser(settings.Format);
        _serialReader = new SerialReader.SerialReader(settings.SerialPort, HandleTelegram);
    }

    /// <inheritdoc/>
    public void Start()
    {
        _serialReader.Start();
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _serialReader.Stop();
    }

    /// <summary>
    /// Handle an incoming telegram message.
    /// </summary>
    /// <param name="telegramMessage">The telegram message to handle.</param>
    private void HandleTelegram(string telegramMessage)
    {
        _logger.LogDebug("Received new message: {message}", telegramMessage);

        //Parse the message.
        var telegram = _parser.ParseTelegram(telegramMessage);
        if (telegram == null)
            return;

        //Update values.
        LastTelegram = telegram;
        TelegramReceived?.Invoke(this, LastTelegram);
    }

    /// <summary>
    /// Get the telegram parser for the given format name.
    /// </summary>
    /// <param name="format">The format name.</param>
    /// <returns>The telegram parser for the given name.</returns>
    private IP1Parser GetParser(string format)
    {
        switch (format)
        {
            case "XS210ESMR5":
                return new XS210ESMR5Parser();
            default:
                _logger.LogError("Unknown format: {format}. Using default format: XS210ESMR5", format);
                return new XS210ESMR5Parser();
        }
    }
}