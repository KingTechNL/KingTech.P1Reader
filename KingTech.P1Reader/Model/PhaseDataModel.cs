namespace KingTech.P1Reader.Model;

public class PhaseDataModel
{
    public double? Voltage { get; set; }
    public long? Current { get; set; }
    public double? PowerDelivered { get; set; }
    public double? PowerReceived { get; set; }
    public long? VoltageSags { get; set; }
    public long? VoltageSwells { get; set; }

}