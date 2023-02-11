namespace KingTech.P1Reader.Models;

/// <summary>
/// Model for the data that is received from the P1 meter.
/// </summary>
public class P1Contract
{
    /// <summary>
    /// The version of the communication protocol used by the P1 connection.
    /// P1 key: 1-3:0.2.8.255
    /// </summary>
    public long? VersionInformation { get; set; }
    /// <summary>
    /// The unique identifier of the device.
    /// P1 key: 0-0:96.1.1.255
    /// </summary>
    public string? EquipmentIdentifier { get; set; }
    /// <summary>
    /// The timestamp of the last measurement in DateTime format.
    /// P1 key: 0-0:1.0.0.255
    /// </summary>
    public DateTime? Timestamp { get; set; }
    /// <summary>
    /// The timestamp of the last measurement as received from the P1 reader.
    /// P1 key: 0-0:1.0.0.255
    /// </summary>
    public string? RawTimestamp { get; set; }

    /// <summary>
    /// Tariff indicator electricity.
    /// The tariff indicator can also be used to switch tariff dependent loads e.g boilers.
    /// This is the responsibility of the P1 user.
    /// P1 key: 0-0:96.14.0.255
    /// </summary>
    public long? TariffIndicator { get; set; }
    /// <summary>
    /// Meter Reading electricity delivered to client (Tariff 1) in 0,001 kWh.
    /// P1 key: 1-0:1.8.1.255
    /// </summary>
    public double? ElectricityUsageLow { get; set; }
    /// <summary>
    /// Meter Reading electricity delivered to client (Tariff 2) in 0,001 kWh.
    /// P1 key: 1-0:1.8.2.255
    /// </summary>
    public double? ElectricityUsageHigh { get; set; }
    /// <summary>
    /// Meter Reading electricity delivered by client (Tariff 1) in 0,001 kWh.
    /// P1 key: 1-0:2.8.1.255
    /// </summary>
    public double? ElectricityReturnedLow { get; set; }
    /// <summary>
    /// Meter Reading electricity delivered by client (Tariff 2) in 0,001 kWh.
    /// P1 key: 1-0:2.8.2.255
    /// </summary>
    public double? ElectricityReturnedHigh { get; set; }
    /// <summary>
    /// Actual electricity power delivered (+P) in 1 Watt resolution.
    /// P1 key: 1-0:1.7.0.255
    /// </summary>
    public double? ActualElectricityDelivered { get; set; }
    /// <summary>
    /// Actual electricity power received (-P) in 1 Watt resolution.
    /// P1 key: 1-0:2.7.0.255
    /// </summary>
    public double? ActualElectricityRetrieved { get; set; }

    /// <summary>
    /// Number of power failures in any phase.
    /// P1 key: 0-0:96.7.21.255
    /// </summary>
    public long? PowerFailuresShort { get; set; }
    /// <summary>
    /// Number of long power failures in any phase.
    /// P1 key: 0-0:96.7.9.255
    /// </summary>
    public long? PowerFailuresLong { get; set; }
    /// <summary>
    /// Power Failure Event Log (long power failures).
    /// P1 key: 1-0:99.97.0.255
    /// </summary>
    public long? PowerFailureEventLog { get; set; }

    /// <summary>
    /// Electricity data for phase L1.
    /// </summary>
    public PhaseDataModel? PhaseL1 { get; set; }
    /// <summary>
    /// Electricity data for phase L2.
    /// </summary>
    public PhaseDataModel? PhaseL2 { get; set; }
    /// <summary>
    /// Electricity data for phase L3.
    /// </summary>
    public PhaseDataModel? PhaseL3 { get; set; }

    /// <summary>
    /// Supplier specific Text message max 1024 characters.
    /// P1 key: 0-0:96.13.0.255
    /// </summary>
    public string? TextMessage { get; set; }

    /// <summary>
    /// Data received for modbus clients.
    /// Multiple <see cref="ModbusClientModel"/> can be connected to the P1 meter.
    /// </summary>
    public IEnumerable<ModbusClientModel>? ModBusClients { get; set; }
}