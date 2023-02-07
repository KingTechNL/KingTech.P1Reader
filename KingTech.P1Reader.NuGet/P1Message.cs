namespace KingTech.P1Reader;

public record P1Message
{
    public DateTime? Timestamp { get; set; }//datetime
    
    public long RawTimestamp { get; set; }//int64

    public double? ElectricityUsageLow { get; set; }//float64

    public double? ElectricityUsageHigh { get; set; }//float64

    public double? ElectricityReturnedLow { get; set; }//float64

    public double? ElectricityReturnedHigh { get; set; }//float64

    public long? ActiveTariff { get; set; }//int64

    public long? PowerFailuresShort { get; set; }//int64

    public long? PowerFailuresLong { get; set; }//int64

    public double? ActualElectricityDelivered { get; set; }//float64

    public double? ActualElectricityRetreived { get; set; }//float64

    public double? GasUsage { get; set; }//float64
}