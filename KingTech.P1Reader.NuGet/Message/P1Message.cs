namespace KingTech.P1Reader.Message;

public class P1Message
{
    public long? VersionInformation { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? RawTimestamp { get; set; }
    public string? EquipmentIdentifier { get; set; }
    public double? ElectricityUsageLow { get; set; }
    public double? ElectricityUsageHigh { get; set; }
    public double? ElectricityReturnedLow { get; set; }
    public double? ElectricityReturnedHigh { get; set; }
    public long? TariffIndicator { get; set; }
    public double? ActualElectricityDelivered { get; set; }
    public double? ActualElectricityRetrieved { get; set; }
    public long? PowerFailuresShort { get; set; }
    public long? PowerFailuresLong { get; set; }
    public long? PowerFailureEventLog { get; set; }
    public long? VoltageSagsL1 { get; set; }
    public long? VoltageSagsL2 { get; set; }
    public long? VoltageSagsL3 { get; set; }
    public long? VoltageSwellsL1 { get; set; }
    public long? VoltageSwellsL2 { get; set; }
    public long? VoltageSwellsL3 { get; set; }
    public string? TextMessage { get; set; }
    public double? VoltageL1 { get; set; }
    public double? VoltageL2 { get; set; }
    public double? VoltageL3 { get; set; }
    public long? CurrentL1 { get; set; }
    public long? CurrentL2 { get; set; }
    public long? CurrentL3 { get; set; }
    public double? PowerDeliveredL1 { get; set; }
    public double? PowerDeliveredL2 { get; set; }
    public double? PowerDeliveredL3 { get; set; }
    public double? PowerReceivedL1 { get; set; }
    public double? PowerReceivedL2 { get; set; }
    public double? PowerReceivedL3 { get; set; }

    public IEnumerable<MBusClient>? MBusClients { get; set; }
}