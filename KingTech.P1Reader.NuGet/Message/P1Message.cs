namespace KingTech.P1Reader.Message;

/// <summary>
/// Record for the data received from the P1 meter.
/// </summary>
public record P1Message
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
    /// Tariff indicator electricity.
    /// The tariff indicator can also be used to switch tariff dependent loads e.g boilers.
    /// This is the responsibility of the P1 user.
    /// P1 key: 0-0:96.14.0.255
    /// </summary>
    public long? TariffIndicator { get; set; }
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
    /// Number of voltage sags in phase L1.
    /// P1 key: 1-0:32.32.0.255
    /// </summary>
    public long? VoltageSagsL1 { get; set; }
    /// <summary>
    /// Number of voltage sags in phase L2.
    /// P1 key: 1-0:52.32.0.255
    /// </summary>
    public long? VoltageSagsL2 { get; set; }
    /// <summary>
    /// Number of voltage sags in phase L3.
    /// P1 key: 1-0:72.32.0.255
    /// </summary>
    public long? VoltageSagsL3 { get; set; }
    /// <summary>
    /// Number of voltage swells in phase L1.
    /// P1 key: 1-0:32.36.0.255
    /// </summary>
    public long? VoltageSwellsL1 { get; set; }
    /// <summary>
    /// Number of voltage swells in phase L2.
    /// P1 key: 1-0:52.36.0.255
    /// </summary>
    public long? VoltageSwellsL2 { get; set; }
    /// <summary>
    /// Number of voltage swells in phase L3.
    /// P1 key: 1-0:72.36.0.255
    /// </summary>
    public long? VoltageSwellsL3 { get; set; }
    /// <summary>
    /// Supplier specific Text message max 1024 characters.
    /// P1 key: 0-0:96.13.0.255
    /// </summary>
    public string? TextMessage { get; set; }
    /// <summary>
    /// Instantaneous voltage for phase L1 in V resolution
    /// P1 key: 1-0:32.7.0.255
    /// </summary>
    public double? VoltageL1 { get; set; }
    /// <summary>
    /// Instantaneous voltage for phase L2 in V resolution
    /// P1 key: 1-0:52.7.0.255
    /// </summary>
    public double? VoltageL2 { get; set; }
    /// <summary>
    /// Instantaneous voltage for phase L3 in V resolution
    /// P1 key: 1-0:72.7.0.255
    /// </summary>
    public double? VoltageL3 { get; set; }
    /// <summary>
    /// Instantaneous current for phase L1 in A resolution.
    /// P1 key: 1-0:31.7.0.255
    /// </summary>
    public long? CurrentL1 { get; set; }
    /// <summary>
    /// Instantaneous current for phase L2 in A resolution.
    /// P1 key: 1-0:51.7.0.255
    /// </summary>
    public long? CurrentL2 { get; set; }
    /// <summary>
    /// Instantaneous current for phase L3 in A resolution.
    /// P1 key: 1-0:71.7.0.255
    /// </summary>
    public long? CurrentL3 { get; set; }
    /// <summary>
    /// Instantaneous active power (+P) for phase L1 in W resolution
    /// P1 key: 1-0:21.7.0.255
    /// </summary>
    public double? PowerDeliveredL1 { get; set; }
    /// <summary>
    /// Instantaneous active power (+P) for phase L2 in W resolution
    /// P1 key: 1-0:41.7.0.255
    /// </summary>
    public double? PowerDeliveredL2 { get; set; }
    /// <summary>
    /// Instantaneous active power (+P) for phase L3 in W resolution
    /// P1 key: 1-0:61.7.0.255
    /// </summary>
    public double? PowerDeliveredL3 { get; set; }
    /// <summary>
    /// Instantaneous active power (-P) for phase L1 in W resolution
    /// P1 key: 1-0:22.7.0.255
    /// </summary>
    public double? PowerReceivedL1 { get; set; }
    /// <summary>
    /// Instantaneous active power (-P) for phase L2 in W resolution
    /// P1 key: 1-0:42.7.0.255
    /// </summary>
    public double? PowerReceivedL2 { get; set; }
    /// <summary>
    /// Instantaneous active power (-P) for phase L3 in W resolution
    /// P1 key: 1-0:62.7.0.255
    /// </summary>
    public double? PowerReceivedL3 { get; set; }

    /// <summary>
    /// Data received for modbus clients. (e.g. gas and/or water meters).
    /// Multiple <see cref="MBusClient"/>s can be connected to the P1 meter.
    /// </summary>
    public IEnumerable<MBusClient>? MBusClients { get; set; }

    /// <summary>
    /// The raw telegram message as received from the P1 port.
    /// </summary>
    public string RawMessage { get; set; } = string.Empty;
}