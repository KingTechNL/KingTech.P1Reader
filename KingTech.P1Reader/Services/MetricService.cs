using Prometheus;

namespace KingTech.P1Reader.Services;

/// <summary>
/// This service adds p1 metrics to the prometheus metrics endpoint.
/// The following metrics will be exposed:
///
/// # HELP p1_active_tariff Active tariff
/// # TYPE p1_active_tariff gauge
/// p1_active_tariff 2
/// # HELP p1_current_usage_electricity_high Electricity currently used high tariff
/// # TYPE p1_current_usage_electricity_high gauge
/// p1_current_usage_electricity_high 0
/// # HELP p1_current_usage_electricity_low Electricity currently used low tariff
/// # TYPE p1_current_usage_electricity_low gauge
/// p1_current_usage_electricity_low 0.2
/// # HELP p1_power_failures_long Power failures long
/// # TYPE p1_power_failures_long gauge
/// p1_power_failures_long 2
/// # HELP p1_power_failures_short Power failures short
/// # TYPE p1_power_failures_short gauge
/// p1_power_failures_short 57
/// # HELP p1_returned_electricity_high Electricity returned high tariff
/// # TYPE p1_returned_electricity_high gauge
/// p1_returned_electricity_high 0
/// # HELP p1_returned_electricity_low Electricity returned low tariff
/// v# TYPE p1_returned_electricity_low gauge
/// p1_returned_electricity_low 0.016
/// # HELP p1_usage_electricity_high Electricity usage high tariff
/// # TYPE p1_usage_electricity_high gauge
/// p1_usage_electricity_high 1225.59
/// # HELP p1_usage_electricity_low Electricity usage low tariff
/// # TYPE p1_usage_electricity_low gauge
/// p1_usage_electricity_low 1179.186
/// # HELP p1_usage_gas Gas usage
/// # TYPE p1_usage_gas gauge
/// p1_usage_gas 1019.003
/// </summary>
public class MetricService
{
    private readonly ILogger<MetricService> _logger;
    private readonly IP1Receiver _receiver;

    private Gauge _activeTariff;
    private Gauge _actualElectricityDelivered;
    private Gauge _actualElectricityRetreived;
    private Gauge _powerFailuresLong;
    private Gauge _powerFailuresShort;
    private Gauge _returnedElectricityHigh;
    private Gauge _returnedElectricityLow;
    private Gauge _usageElectricityHigh;
    private Gauge _usageElectricityLow;
    private Gauge _usageGas;

    public MetricService(ILogger<MetricService> logger, IP1Receiver receiver)
    {
        _logger = logger;
        _receiver = receiver;

        AddMetrics();
    }

    public void Start()
    {
        _receiver.TelegramReceived += SetValues;
    }

    public void Stop()
    {
        _receiver.TelegramReceived -= SetValues;
    }

    /// <summary>
    /// Set values for metric endpoints.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="p1Message"></param>
    private void SetValues(object? sender, P1Message p1Message)
    {

        _logger.LogTrace("Setting values for metrics");
        _activeTariff.Set(p1Message.ActiveTariff);
        _actualElectricityDelivered.Set(p1Message.ActualElectricityDelivered);
        _actualElectricityRetreived.Set(p1Message.ActualElectricityRetreived);
        _powerFailuresLong.Set(p1Message.PowerFailuresLong);
        _powerFailuresShort.Set(p1Message.PowerFailuresShort);
        _returnedElectricityHigh.Set(p1Message.ElectricityReturnedHigh);
        _returnedElectricityLow.Set(p1Message.ElectricityReturnedLow);
        _usageElectricityHigh.Set(p1Message.ElectricityUsageHigh);
        _usageElectricityLow.Set(p1Message.ElectricityUsageLow);
        _usageGas.Set(p1Message.GasUsage);
    }

    /// <summary>
    /// Add new metric endpoints.
    /// </summary>
    private void AddMetrics()
    {
        _logger.LogTrace("Adding metric endpoints for P1 values.");
        _activeTariff = Metrics.CreateGauge("p1_active_tariff", "Active tariff");
        _actualElectricityDelivered = Metrics.CreateGauge("p1_actual_electricity_delivered", "Electricity actually delivered");
        _actualElectricityRetreived = Metrics.CreateGauge("p1_actual_electricity_retreived", "Electricity actually retreived");
        _powerFailuresLong = Metrics.CreateGauge("p1_power_failures_long", "Power failures long");
        _powerFailuresShort = Metrics.CreateGauge("p1_power_failures_short", "Power failures short");
        _returnedElectricityHigh = Metrics.CreateGauge("p1_returned_electricity_high", "Electricity returned high tariff");
        _returnedElectricityLow = Metrics.CreateGauge("p1_returned_electricity_low", "Electricity returned low tariff");
        _usageElectricityHigh = Metrics.CreateGauge("p1_usage_electricity_high", "Electricity usage high tariff");
        _usageElectricityLow = Metrics.CreateGauge("p1_usage_electricity_low", "Electricity usage low tariff");
        _usageGas = Metrics.CreateGauge("p1_usage_gas", "Gas usage");
    }
}