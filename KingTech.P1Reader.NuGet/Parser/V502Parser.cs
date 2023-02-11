using System.Text.RegularExpressions;
using KingTech.P1Reader.Message;

namespace KingTech.P1Reader.Parser;

/// <summary>
/// Parser for P1 v5.0.2 messages.
/// Example:
///
/// /ISk5\2MT382-1000
/// 1-3:0.2.8(50)
/// 0-0:1.0.0(101209113020W)
/// 0-0:96.1.1(4B384547303034303436333935353037)
/// 1-0:1.8.1(123456.789* kWh)
/// 1-0:1.8.2(123456.789* kWh)
/// 1-0:2.8.1(123456.789* kWh)
/// 1-0:2.8.2(123456.789* kWh)
/// 0-0:96.14.0(0002)
/// 1-0:1.7.0(01.193* kW)
/// 1-0:2.7.0(00.000* kW)
/// 0-0:96.7.21(00004)
/// 0-0:96.7.9(00002)
/// 1-0:99.97.0(2)(0-0:96.7.19)(101208152415W)(0000000240* s)(101208151004W)(0000000301* s)
/// 1-0:32.32.0(00002)
/// 1-0:52.32.0(00001)
/// 1-0:72.32.0(00000)
/// 1-0:32.36.0(00000)
/// 1-0:52.36.0(00003)
/// 1-0:72.36.0(00000)
/// 0-0:96.13.0(303132333435363738393A3B3C3D3E3F303132333435363738393A3B3C3D3E3F303132333435363738393A3B3C3D3E3F303132333435363738393A3B3C3D3E3F303132333435363738393A3B3C3D3E3F)
/// 1-0:32.7.0(220.1* V)
/// 1-0:52.7.0(220.2* V)
/// 1-0:72.7.0(220.3* V)
/// 1-0:31.7.0(001* A)
/// 1-0:51.7.0(002* A)
/// 1-0:71.7.0(003* A)
/// 1-0:21.7.0(01.111* kW)
/// 1-0:41.7.0(02.222* kW)
/// 1-0:61.7.0(03.333* kW)
/// 1-0:22.7.0(04.444* kW)
/// 1-0:42.7.0(05.555* kW)
/// 1-0:62.7.0(06.666* kW)
/// 0-1:24.1.0(003)
/// 0-1:96.1.0(3232323241424344313233343536373839)
/// 0-1:24.2.1(101209112500W)(12785.123* m3)
/// !EF2F
/// </summary>
internal class V502Parser : ABaseP1Parser
{
    internal const string KeyVersionInformation = "1-3:0.2.8";
    internal const string KeyTimestamp = "0-0:1.0.0";
    internal const string KeyEquipmentIdentifier = "0-0:96.1.1";
    internal const string KeyElectricityUsageLow = "1-0:1.8.1"; //Low = Tarif1
    internal const string KeyElectricityUsageHigh = "1-0:1.8.2"; //High = Tarif2
    internal const string KeyElectricityReturnedLow = "1-0:2.8.1";
    internal const string KeyElectricityReturnedHigh = "1-0:2.8.2";
    internal const string KeyTariffIndicator = "0-0:96.14.0";
    internal const string KeyActualElectricityDelivered = "1-0:1.7.0";
    internal const string KeyActualElectricityRetrieved = "1-0:2.7.0";
    internal const string KeyPowerFailuresShort = "0-0:96.7.21";
    internal const string KeyPowerFailuresLong = "0-0:96.7.9";
    internal const string KeyPowerFailureEventLog = "1-0:99.97.0";
    internal const string KeyVoltageSagsL1 = "1-0:32.32.0";
    internal const string KeyVoltageSagsL2 = "1-0:52.32.0";
    internal const string KeyVoltageSagsL3 = "1-0:72.32.0";
    internal const string KeyVoltageSwellsL1 = "1-0:32.36.0";
    internal const string KeyVoltageSwellsL2 = "1-0:52.36.0";
    internal const string KeyVoltageSwellsL3 = "1-0:72.36.0";
    internal const string KeyTextMessage = "0-0:96.13.0";
    internal const string KeyVoltageL1 = "1-0:32.7.0";
    internal const string KeyVoltageL2 = "1-0:52.7.0";
    internal const string KeyVoltageL3 = "1-0:72.7.0";
    internal const string KeyCurrentL1 = "1-0:31.7.0";
    internal const string KeyCurrentL2 = "1-0:51.7.0";
    internal const string KeyCurrentL3 = "1-0:71.7.0";
    internal const string KeyPowerDeliveredL1 = "1-0:21.7.0";
    internal const string KeyPowerDeliveredL2 = "1-0:41.7.0";
    internal const string KeyPowerDeliveredL3 = "1-0:61.7.0";
    internal const string KeyPowerReceivedL1 = "1-0:22.7.0";
    internal const string KeyPowerReceivedL2 = "1-0:42.7.0";
    internal const string KeyPowerReceivedL3 = "1-0:62.7.0";

    private Regex KeyMBusReading    = new("0-(.*):24\\.2\\.1");
    private Regex KeyMBusDeviceType = new("0-(.*):24\\.1\\.0");
    private Regex KeyMBusDeviceId   = new("0-(.*):96\\.1\\.0");

    /// <inheritdoc />
    public override P1Message? ParseTelegram(string telegram)
    {
        var telegramLines = ParseTelegramValues(telegram);

        if (telegramLines.Count > 0)
        {
            return new P1Message()
            {
                VersionInformation = ParseLong(telegramLines, KeyVersionInformation),
                Timestamp = ParseTimestamp(telegramLines, KeyTimestamp),
                RawTimestamp = ParseString(telegramLines, KeyTimestamp),
                EquipmentIdentifier = ParseString(telegramLines, KeyEquipmentIdentifier),
                ElectricityUsageLow = ParseDouble(telegramLines, KeyElectricityUsageLow),
                ElectricityUsageHigh = ParseDouble(telegramLines, KeyElectricityUsageHigh),
                ElectricityReturnedLow = ParseDouble(telegramLines, KeyElectricityReturnedLow),
                ElectricityReturnedHigh = ParseDouble(telegramLines, KeyElectricityReturnedHigh),
                TariffIndicator = ParseLong(telegramLines, KeyTariffIndicator),
                ActualElectricityDelivered = ParseDouble(telegramLines, KeyActualElectricityDelivered),
                ActualElectricityRetrieved = ParseDouble(telegramLines, KeyActualElectricityRetrieved),
                PowerFailuresShort = ParseLong(telegramLines, KeyPowerFailuresShort),
                PowerFailuresLong = ParseLong(telegramLines, KeyPowerFailuresLong),
                //PowerFailureEventLog = ParseLong(telegramLines, KeyPowerFailureEventLog, 2),
                VoltageSagsL1 = ParseLong(telegramLines, KeyVoltageSagsL1),
                VoltageSagsL2 = ParseLong(telegramLines, KeyVoltageSagsL2),
                VoltageSagsL3 = ParseLong(telegramLines, KeyVoltageSagsL3),
                VoltageSwellsL1 = ParseLong(telegramLines, KeyVoltageSwellsL1),
                VoltageSwellsL2 = ParseLong(telegramLines, KeyVoltageSwellsL2),
                VoltageSwellsL3 = ParseLong(telegramLines, KeyVoltageSwellsL3),
                TextMessage = ParseString(telegramLines, KeyTextMessage),
                VoltageL1 = ParseDouble(telegramLines, KeyVoltageL1),
                VoltageL2 = ParseDouble(telegramLines, KeyVoltageL2),
                VoltageL3 = ParseDouble(telegramLines, KeyVoltageL3),
                CurrentL1 = ParseLong(telegramLines, KeyCurrentL1),
                CurrentL2 = ParseLong(telegramLines, KeyCurrentL2),
                CurrentL3 = ParseLong(telegramLines, KeyCurrentL3),
                PowerDeliveredL1 = ParseDouble(telegramLines, KeyPowerDeliveredL1),
                PowerDeliveredL2 = ParseDouble(telegramLines, KeyPowerDeliveredL2),
                PowerDeliveredL3 = ParseDouble(telegramLines, KeyPowerDeliveredL3),
                PowerReceivedL1 = ParseDouble(telegramLines, KeyPowerReceivedL1),
                PowerReceivedL2 = ParseDouble(telegramLines, KeyPowerReceivedL2),
                PowerReceivedL3 = ParseDouble(telegramLines, KeyPowerReceivedL3),
                MBusClients = GetMBusClients(telegramLines),
                RawMessage = telegram
            };
        }

        return null;
    }

    /// <summary>
    /// Get all modbus clients from the telegram message.
    /// </summary>
    /// <param name="telegramLines">The telegram lines parsed from the incoming message string.</param>
    /// <returns>The <see cref="MBusClient"/>s in the given telegram.</returns>
    private IEnumerable<MBusClient> GetMBusClients(Dictionary<string, List<string>> telegramLines)
    {
        //Get all device ID's
        var mbusDevices = (from key in telegramLines.Keys select KeyMBusDeviceId.Match(key) into match where match.Success select match.Groups[1].Value).ToList();
        mbusDevices.AddRange(from key in telegramLines.Keys select KeyMBusDeviceType.Match(key) into match where match.Success select match.Groups[1].Value);
        mbusDevices.AddRange(from key in telegramLines.Keys select KeyMBusReading.Match(key) into match where match.Success select match.Groups[1].Value);

        var x = GetMbusDeviceIndexes(telegramLines.Keys.ToList(), KeyMBusDeviceId);

        //Get all client data for each device.
        return mbusDevices.Distinct()
            .Select(device => new MBusClient()
            {
                DeviceIdentifier = ParseString(telegramLines, $"0-{device}:96.1.0"),
                DeviceType = ParseLong(telegramLines, $"0-{device}:24.1.0"),
                CaptureTime = ParseTimestamp(telegramLines, $"0-{device}:24.2.1"),
                RawCaptureTime = ParseLong(telegramLines, $"0-{device}:24.2.1"),
                Value = ParseDouble(telegramLines, $"0-{device}:24.2.1", 1)
            });
    }

    /// <summary>
    /// Get the indexes of all modbus devices.
    /// </summary>
    /// <param name="keys">A list of all keys in the P1 message.</param>
    /// <param name="regex">The regex to isolate the index in the given keys (in group 1)</param>
    /// <returns>A list of all indexes for the modbus devices.</returns>
    private List<string> GetMbusDeviceIndexes(List<string> keys, Regex regex)
    {
        var indexes = new List<string>();
        foreach (var key in keys)
        {
            var match = regex.Match(key);
            if (match.Success)
            {
                indexes.Add(match.Groups[1].Name);
            }
        }
        return indexes;
    }
}