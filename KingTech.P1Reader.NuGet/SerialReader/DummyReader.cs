using System.Text;

namespace KingTech.P1Reader.SerialReader;

public class DummyReader : ISerialReader
{
    private Action<string> _onMessageReceived;
    private Thread _thread;
    private bool _isRunning = false;

    public DummyReader(Action<string> onMessageReceived)
    {
        _onMessageReceived = onMessageReceived;
    }
    
    public bool Start()
    {
        _isRunning = true;
        _thread = new Thread(CreateDummyMessage);
        _thread.Start();
        return true;
    }

    public bool Stop()
    {
        _isRunning = false;
        _thread.Join();
        return true;
    }

    private void CreateDummyMessage()
    {
        while (_isRunning)
        {
            //Create dummy message.
            var sb = new StringBuilder();
            sb.AppendLine("/KFM5KAIFA-METER");
            sb.AppendLine("1-3:0.2.8(50)");
            sb.AppendLine("0-0:1.0.0(230205165005W)");
            sb.AppendLine("0-0:96.1.1(4530303435303033383039333631343137)");
            sb.AppendLine("1-0:1.8.1(011080.001*kWh)");
            sb.AppendLine("1-0:1.8.2(011931.124*kWh)");
            sb.AppendLine("1-0:2.8.1(000914.942*kWh)");
            sb.AppendLine("1-0:2.8.2(001837.962*kWh)");
            sb.AppendLine("0-0:96.14.0(0001)");
            sb.AppendLine("1-0:1.7.0(00.404*kW)");
            sb.AppendLine("1-0:2.7.0(00.000*kW)");
            sb.AppendLine("0-0:96.7.21(00007)");
            sb.AppendLine("0-0:96.7.9(00009)");
            sb.AppendLine("1-0:99.97.0(9)(0-0:96.7.19)(190315191821W)(0000005100*s)(190521113136S)(0000000516*s)(211014073208S)(0000000615*s)(211022160832S)(0000005232*s)(211222113427W)(0000001441*s)(220331141855S)(0000008493*s)(220915122840S)(0000001629*s)(220915132057S)(0000002966*s)(221009193502S)(0000027164*s)");
            sb.AppendLine("1-0:32.32.0(00046)");
            sb.AppendLine("1-0:32.36.0(00014)");
            sb.AppendLine("0-0:96.13.0()");
            sb.AppendLine("1-0:32.7.0(229.0*V)");
            sb.AppendLine("1-0:31.7.0(003*A)");
            sb.AppendLine("1-0:21.7.0(00.404*kW)");
            sb.AppendLine("1-0:22.7.0(00.000*kW)");
            sb.AppendLine("0-2:24.1.0(003)");
            sb.AppendLine("0-2:96.1.0(4730303332353635353433363436313137)");
            sb.AppendLine("0-2:24.2.1(230205164505W)(06919.956*m3)");
            sb.AppendLine("!8FF3");
            _onMessageReceived(sb.ToString());
            Thread.Sleep(5000);
        }
    }
}