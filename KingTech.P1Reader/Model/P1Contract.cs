namespace KingTech.P1Reader.Model;

public class P1Contract
{
    //Message meta data
    public long? VersionInformation { get; set; }
    public string? EquipmentIdentifier { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? RawTimestamp { get; set; }

    //General electricity data
    public long? TariffIndicator { get; set; }
    public double? ElectricityUsageLow { get; set; }
    public double? ElectricityUsageHigh { get; set; }
    public double? ElectricityReturnedLow { get; set; }
    public double? ElectricityReturnedHigh { get; set; }
    public double? ActualElectricityDelivered { get; set; }
    public double? ActualElectricityRetrieved { get; set; }

    //Power failures
    public long? PowerFailuresShort { get; set; }
    public long? PowerFailuresLong { get; set; }
    public long? PowerFailureEventLog { get; set; }

    //Phase specific data.
    public PhaseDataModel? PhaseL1 { get; set; }
    public PhaseDataModel? PhaseL2 { get; set; }
    public PhaseDataModel? PhaseL3 { get; set; }

    //Custom text message
    public string? TextMessage { get; set; }

    //Often used for gas meters, trails with water meters are ongoing.
    public IEnumerable<ModbusClientModel>? ModBusClients { get; set; }
}