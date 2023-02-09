using KingTech.P1Reader.Message;
using KingTech.P1Reader.Parser;
using KingTech.P1Reader.SerialReader;
using Microsoft.Extensions.Logging;
using static KingTech.P1Reader.IP1Receiver;

namespace KingTech.P1Reader;

/// <inheritdoc/>
public class P1Receiver : IP1Receiver
{
    /// <inheritdoc/>
    public P1Message? LastTelegram { get; private set; }

    /// <inheritdoc/>
    public TelegramReceivedEvent OnTelegramReceived { get; set; }

    private readonly ILogger<P1Receiver>? _logger;
    private readonly IP1Parser _parser;

    private readonly ISerialReader _serialReader;

    /// <summary>
    /// The P1 receiver is responsible for receiving and parsing P1 messages using the given configuration.
    /// </summary>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create loggers for the P1Receiver system.</param>
    /// <param name="settings">The <see cref="P1ReceiverSettings"/> for this receiver.</param>
    public P1Receiver(ILoggerFactory? loggerFactory, P1ReceiverSettings settings)
    {
        _logger = loggerFactory?.CreateLogger<P1Receiver>();

        _parser = GetParser(settings.Format);
        _serialReader = settings.SerialPort.ToUpper() == "DUMMY" 
            ? new SerialReader.DummyReader(loggerFactory?.CreateLogger<DummyReader>(), HandleTelegram)
            : new SerialReader.SerialReader(loggerFactory?.CreateLogger<SerialReader.SerialReader>(), settings.SerialPort, HandleTelegram);
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
        _logger?.LogDebug("Received new message: {message}", telegramMessage);

        //Parse the message.
        var telegram = _parser.ParseTelegram(telegramMessage);
        if (telegram == null)
            return;

        //Update values.
        LastTelegram = telegram;

        OnTelegramReceived?.Invoke(LastTelegram).Wait();
    }

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
                _logger?.LogError("Unknown format: {format}. Using default format: v502", format);
                return new V502Parser();
        }
    }
}