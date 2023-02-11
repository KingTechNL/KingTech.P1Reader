namespace KingTech.P1Reader.Models;

/// <summary>
/// Electricity measurements for a single phase.
/// </summary>
public class PhaseDataModel
{
    /// <summary>
    /// Instantaneous voltage for this phase in V resolution
    /// P1 key L1: 1-0:32.7.0.255
    /// P1 key L2: 1-0:52.7.0.255
    /// P1 key L3: 1-0:72.7.0.255
    /// </summary>
    public double? Voltage { get; set; }
    /// <summary>
    /// Instantaneous current for this phase in A resolution.
    /// P1 key L1: 1-0:31.7.0.255
    /// P1 key L2: 1-0:51.7.0.255
    /// P1 key L3: 1-0:71.7.0.255
    /// </summary>
    public long? Current { get; set; }
    /// <summary>
    /// Instantaneous active power for this phase (+P) in W resolution
    /// P1 key L1: 1-0:21.7.0.255
    /// P1 key L2: 1-0:41.7.0.255
    /// P1 key L3: 1-0:61.7.0.255
    /// </summary>
    public double? PowerDelivered { get; set; }
    /// <summary>
    /// Instantaneous active power for this phase (-P) in W resolution
    /// P1 key L1: 1-0:22.7.0.255
    /// P1 key L2: 1-0:42.7.0.255
    /// P1 key L3: 1-0:62.7.0.255
    /// </summary>
    public double? PowerReceived { get; set; }
    /// <summary>
    /// Number of voltage sags in this phase.
    /// P1 key L1: 1-0:32.32.0.255
    /// P1 key L2: 1-0:52.32.0.255
    /// P1 key L3: 1-0:72.32.0.255
    /// </summary>
    public long? VoltageSags { get; set; }
    /// <summary>
    /// Number of voltage swells in this phase.
    /// P1 key L1: 1-0:32.36.0.255
    /// P1 key L2: 1-0:52.36.0.255
    /// P1 key L3: 1-0:72.36.0.255
    /// </summary>
    public long? VoltageSwells { get; set; }

}