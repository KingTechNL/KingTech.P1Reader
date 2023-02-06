using System.IO.Ports;
using System.Text;

namespace KingTech.P1Reader.SerialReader;

internal class SerialReader
{
    private SerialPort _serialPort;
    private Action<string> _onMessageReceived;

    private StringBuilder _messageBuilder;

    public SerialReader(string portName, Action<string> onMessageReceived)
    {
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
    }

    public bool Start()
    {
        try
        {
            _serialPort.Open();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public bool Stop()
    {
        try
        {
            _serialPort.Close();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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
            _onMessageReceived?.Invoke(_messageBuilder.ToString());
            _messageBuilder.Clear();
        }
    }
}