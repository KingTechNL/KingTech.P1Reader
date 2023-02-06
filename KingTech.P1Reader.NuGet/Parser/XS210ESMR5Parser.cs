namespace KingTech.P1Reader.Parser;

/// <summary>
/// Parser for XS210ESMR5 messages.
/// XS210ESMR5 messages look like this:
/// 
/// /XMX5<meter id>
/// 1-3:0.2.8(50)
/// 0-0:1.0.0(180103220322W)
/// 0-0:96.1.1(<equipment id>)
/// 1-0:1.8.1(000143.422* kWh)
/// 1-0:1.8.2(000078.911* kWh)
/// 1-0:2.8.1(000000.000* kWh)
/// 1-0:2.8.2(000000.000* kWh)
/// 0-0:96.14.0(0002)
/// 1-0:1.7.0(00.520* kW)
/// 1-0:2.7.0(00.000* kW)
/// 0-0:96.7.21(00003)
/// 0-0:96.7.9(00000)
/// 1-0:99.97.0(0)(0-0:96.7.19)
/// 1-0:32.32.0(00000)
/// 1-0:32.36.0(00000)
/// 0-0:96.13.0()
/// 1-0:32.7.0(233.0* V)
/// 1-0:31.7.0(002* A)
/// 1-0:21.7.0(00.520* kW)
/// 1-0:22.7.0(00.000* kW)
/// !9384
/// </summary>
internal class XS210ESMR5Parser : ABaseP1Parser
{
    internal const string KeyTimestamp = "0-0:1.0.0";
    internal const string KeyElectricityUsageLow = "1-0:1.8.1";
    internal const string KeyElectricityUsageHigh = "1-0:1.8.2";
    internal const string KeyElectricityReturnedLow = "1-0:2.8.1";
    internal const string KeyElectricityReturnedHigh = "1-0:2.8.2";
    internal const string KeyActiveTariff = "0-0:96.14.0";
    internal const string KeyActualElectricityDelivered = "1-0:1.7.0";
    internal const string KeyActualElectricityRetreived = "1-0:2.7.0";
    internal const string KeyPowerFailuresShort = "0-0:96.7.21";
    internal const string KeyPowerFailuresLong = "0-0:96.7.9";
    internal const string KeyGasUsage = "0-2:24.2.1"; //"0-1:24.2.1";

    /// <inheritdoc />
    public override P1Message? ParseTelegram(string telegram)
    {
        var telegramLines = ParseTelegramValues(telegram);

        if (telegramLines.Count > 0)
        {
            var timestamp = ParseTimestamp(telegramLines, KeyTimestamp);
            var rawTimestamp = ParseLong(telegramLines, KeyTimestamp);
            var electricityUsageHigh = ParseDouble(telegramLines, KeyElectricityUsageHigh);
            var electricityUsageLow = ParseDouble(telegramLines, KeyElectricityUsageLow);
            var electricityReturnedHigh = ParseDouble(telegramLines, KeyElectricityReturnedHigh);
            var electricityReturnedLow = ParseDouble(telegramLines, KeyElectricityReturnedLow);
            var activeTariff = ParseLong(telegramLines, KeyActiveTariff);
            var powerFailuresLong = ParseLong(telegramLines, KeyPowerFailuresLong);
            var powerFailureShort = ParseLong(telegramLines, KeyPowerFailuresShort);
            var actualElectricityDelivered = ParseDouble(telegramLines, KeyActualElectricityDelivered);
            var actualElectricityRetreived = ParseDouble(telegramLines, KeyActualElectricityRetreived);
            var gasUsage = ParseDouble(telegramLines, KeyGasUsage, 1);

            return new P1Message()
            {
                Timestamp = timestamp,
                RawTimestamp = rawTimestamp,
                ElectricityUsageHigh = electricityUsageHigh,
                ElectricityUsageLow = electricityUsageLow,
                ElectricityReturnedHigh = electricityReturnedHigh,
                ElectricityReturnedLow = electricityReturnedLow,
                ActiveTariff = activeTariff,
                PowerFailuresLong = powerFailuresLong,
                PowerFailuresShort = powerFailureShort,
                ActualElectricityDelivered = actualElectricityDelivered,
                ActualElectricityRetreived = actualElectricityRetreived,
                GasUsage = gasUsage,
            };
        }

        return null;
    }
}