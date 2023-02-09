﻿using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Logging;

namespace KingTech.P1Reader.SerialReader;

internal class SerialReader : ISerialReader
{
    private SerialPort _serialPort;
    private readonly ILogger<SerialReader>? _logger;
    private Action<string> _onMessageReceived;

    private StringBuilder _messageBuilder;

    public SerialReader(ILogger<SerialReader>? logger, string portName, Action<string> onMessageReceived)
    {
        _logger = logger;
        _onMessageReceived = onMessageReceived;
        _messageBuilder = new StringBuilder();

        //Setup serial connection.
        _serialPort = new SerialPort(portName);
        _serialPort.BaudRate = 115200;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.DataBits = 8;
        _serialPort.Handshake = Handshake.None;
        _serialPort.DataReceived += DataReceivedHandler;
        _logger?.LogInformation("Serial reader initialized for port {@SerialPort}.", _serialPort);
    }

    public bool Start()
    {
        try
        {
            _logger?.LogInformation("Opening connection to serial port {@SerialPort}.", _serialPort);
            _serialPort.Open();
            _logger?.LogInformation("Serial reader started for port {@SerialPort}.", _serialPort);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to open connection to serial port {@SerialPort}", _serialPort);
            return false;
        }
    }

    public bool Stop()
    {
        try
        {
            _logger?.LogInformation("Closing connection to serial port {@SerialPort}.", _serialPort);
            _serialPort.Close();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to gracefully close serial connection.");
            return false;
        }
    }

    /// <summary>
    /// Event handler for serial data received.
    /// Read the data and pass it to the OnMessageReceived event.
    /// </summary>
    /// <param name="sender">The serial port.</param>
    /// <param name="e"></param>
    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        var sp = (SerialPort)sender;
        var indata = sp.ReadExisting();

        _messageBuilder.Append(indata);

        //Check if the message is complete.
        if (indata.Contains('!'))
        {
            //Message is complete, pass it to the event.
            var message = _messageBuilder.ToString();
            _logger?.LogTrace("Received message from serial port {@SerialPort}: {@Message}", 
                _serialPort, message);
            _onMessageReceived?.Invoke(message);
            _messageBuilder.Clear();
        }
    }
}