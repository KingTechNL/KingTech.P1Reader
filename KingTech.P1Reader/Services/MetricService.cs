using KingTech.P1Reader.Broker;
using KingTech.P1Reader.Message;
using Prometheus;

namespace KingTech.P1Reader.Services;

/// <summary>
/// This service adds p1 metrics to the prometheus metrics endpoint.
/// A unique metric is added for every numeric value in the P1 spec.
/// </summary>
public class MetricService
{
    private readonly ILogger<MetricService> _logger;
    private readonly IMessageBroker<P1Message> _messageBroker;

    private Gauge _versionInformation;
    private Gauge _timestamp;
    private Gauge _electricityUsageHigh;
    private Gauge _electricityUsageLow;
    private Gauge _electricityReturnedHigh;
    private Gauge _electricityReturnedLow;
    private Gauge _tariffIndicator;
    private Gauge _actualElectricityDelivered;
    private Gauge _actualElectricityRetrieved;
    private Gauge _powerFailuresLong;
    private Gauge _powerFailuresShort;
    private Gauge _voltageSagsL1;
    private Gauge _voltageSagsL2;
    private Gauge _voltageSagsL3;
    private Gauge _voltageSwellsL1;
    private Gauge _voltageSwellsL2;
    private Gauge _voltageSwellsL3;
    private Gauge _voltageL1;
    private Gauge _voltageL2;
    private Gauge _voltageL3;
    private Gauge _currentL1;
    private Gauge _currentL2;
    private Gauge _currentL3;
    private Gauge _powerDeliveredL1;
    private Gauge _powerDeliveredL2;
    private Gauge _powerDeliveredL3;
    private Gauge _powerReceivedL1;
    private Gauge _powerReceivedL2;
    private Gauge _powerReceivedL3;

    private Dictionary<string, ModbusMetrics> _modbusMetrics = new();

    /// <summary>
    /// This service adds p1 metrics to the prometheus metrics endpoint.
    /// A unique metric is added for every numeric value in the P1 spec.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> for this service.</param>
    /// <param name="messageBroker">The <see cref="IP1Receiver"/> to subscribe to new P1 messages.</param>
    public MetricService(ILogger<MetricService> logger, IMessageBroker<P1Message> messageBroker)
    {
        _logger = logger;
        _messageBroker = messageBroker;

        AddMetrics();
    }

    /// <summary>
    /// Start listening for P1 messages and update metric values accordingly.
    /// </summary>
    public void Start()
    {
        _messageBroker.Subscribe(SetValues);
    }

    /// <summary>
    /// Stop listening for P1 messages.
    /// </summary>
    public void Stop()
    {
        _messageBroker.Unsubscribe(SetValues);
    }

    /// <summary>
    /// Set values for metric endpoints.
    /// </summary>
    /// <param name="p1Message"></param>
    private void SetValues(P1Message? p1Message)
    {
        if (p1Message == null)
            return;

        _logger.LogTrace("Setting values for metrics");

        var timestamp = p1Message.Timestamp ?? DateTime.Now;
        _timestamp.Set(timestamp.ToFileTime());
        
        TrySet(_versionInformation, p1Message.VersionInformation);
        
        TrySet(_tariffIndicator, p1Message.TariffIndicator);

        TrySet(_electricityUsageHigh, p1Message.ElectricityUsageHigh);
        TrySet(_electricityUsageLow, p1Message.ElectricityUsageLow);
        
        TrySet(_electricityReturnedHigh, p1Message.ElectricityReturnedHigh);
        TrySet(_electricityReturnedLow, p1Message.ElectricityReturnedLow);

        TrySet(_actualElectricityDelivered, p1Message.ActualElectricityDelivered);
        TrySet(_actualElectricityRetrieved, p1Message.ActualElectricityRetrieved);
        
        TrySet(_powerFailuresLong, p1Message.PowerFailuresLong);
        TrySet(_powerFailuresShort, p1Message.PowerFailuresShort);
        
        TrySet(_voltageSagsL1, p1Message.VoltageSagsL1);
        TrySet(_voltageSagsL2, p1Message.VoltageSagsL2);
        TrySet(_voltageSagsL3, p1Message.VoltageSagsL3);

        TrySet(_voltageSwellsL1, p1Message.VoltageSwellsL1);
        TrySet(_voltageSwellsL2, p1Message.VoltageSwellsL2);
        TrySet(_voltageSwellsL3, p1Message.VoltageSwellsL3);

        TrySet(_voltageL1, p1Message.VoltageL1);
        TrySet(_voltageL2, p1Message.VoltageL1);
        TrySet(_voltageL3, p1Message.VoltageL3);

        TrySet(_currentL1, p1Message.CurrentL1);
        TrySet(_currentL2, p1Message.CurrentL2);
        TrySet(_currentL3, p1Message.CurrentL3);

        TrySet(_powerDeliveredL1, p1Message.PowerDeliveredL1);
        TrySet(_powerDeliveredL2, p1Message.PowerDeliveredL2);
        TrySet(_powerDeliveredL3, p1Message.PowerDeliveredL3);

        TrySet(_powerReceivedL1, p1Message.PowerReceivedL1);
        TrySet(_powerReceivedL2, p1Message.PowerReceivedL2);
        TrySet(_powerReceivedL3, p1Message.PowerReceivedL3);

        //Handle all modbus clients.
        if (p1Message.MBusClients != null)
        {
            SetModbusMetrics(p1Message);
        }
    }

    /// <summary>
    /// Add new metric endpoints.
    /// </summary>
    private void AddMetrics()
    {
        _logger.LogTrace("Adding metric endpoints for P1 values.");

        _versionInformation = Metrics.CreateGauge("p1_version", "Version of the P1 protocol used");
        _timestamp = Metrics.CreateGauge("p1_timestamp", "Timestamp of the last known P1 message, format: YYMMDDhhmmssX");
        _electricityUsageHigh = Metrics.CreateGauge("p1_usage_electricity_high", "Electricity usage high tariff in 0,001 kWh");
        _electricityUsageLow = Metrics.CreateGauge("p1_usage_electricity_low", "Electricity usage low tariff in 0,001 kWh");
        _electricityReturnedHigh = Metrics.CreateGauge("p1_returned_electricity_high", "Electricity returned high tariff in 0,001 kWh");
        _electricityReturnedLow = Metrics.CreateGauge("p1_returned_electricity_low", "Electricity returned low tariff in 0,001 kWh");
        _tariffIndicator = Metrics.CreateGauge("p1_tariff_indicator", "Active tariff");
        _actualElectricityDelivered = Metrics.CreateGauge("p1_actual_electricity_delivered", "Actual electricity power delivered (+P) in 1 Watt resolution");
        _actualElectricityRetrieved = Metrics.CreateGauge("p1_actual_electricity_retrieved", "Actual electricity power received (-P) in 1 Watt resolution");
        _powerFailuresLong = Metrics.CreateGauge("p1_power_failures_long", "Number of power failures in any phase");
        _powerFailuresShort = Metrics.CreateGauge("p1_power_failures_short", "Number of long power failures in any phase");
        _voltageSagsL1 = Metrics.CreateGauge("p1_voltage_sags_l1", "Number of voltage sags in phase L1");
        _voltageSagsL2 = Metrics.CreateGauge("p1_voltage_sags_l2", "Number of voltage sags in phase L2");
        _voltageSagsL3 = Metrics.CreateGauge("p1_voltage_sags_l3", "Number of voltage sags in phase L3");
        _voltageSwellsL1 = Metrics.CreateGauge("p1_voltage_swells_l1", "Number of voltage swells in phase L1");
        _voltageSwellsL2 = Metrics.CreateGauge("p1_voltage_swells_l2", "Number of voltage swells in phase L2");
        _voltageSwellsL3 = Metrics.CreateGauge("p1_voltage_swells_l3", "Number of voltage swells in phase L3");
        _voltageL1 = Metrics.CreateGauge("p1_voltage_l1", "Instantaneous voltage for phase L1 in V resolution");
        _voltageL2 = Metrics.CreateGauge("p1_voltage_l2", "Instantaneous voltage for phase L2 in V resolution");
        _voltageL3 = Metrics.CreateGauge("p1_voltage_l3", "Instantaneous voltage for phase L3 in V resolution");
        _currentL1 = Metrics.CreateGauge("p1_current_l1", "Instantaneous current for phase L1 in A resolution");
        _currentL2 = Metrics.CreateGauge("p1_current_l2", "Instantaneous current for phase L2 in A resolution");
        _currentL3 = Metrics.CreateGauge("p1_current_l3", "Instantaneous current for phase L3 in A resolution");
        _powerDeliveredL1 = Metrics.CreateGauge("p1_power_delivered_l1", "Instantaneous active power delivered to phase L1 (-P) in W resolution");
        _powerDeliveredL2 = Metrics.CreateGauge("p1_power_delivered_l2", "Instantaneous active power delivered to phase L2 (-P) in W resolution");
        _powerDeliveredL3 = Metrics.CreateGauge("p1_power_delivered_l3", "Instantaneous active power delivered to phase L3 (-P) in W resolution");
        _powerReceivedL1 = Metrics.CreateGauge("p1_power_received_l1", "Instantaneous active power received for phase L1 (-P) in W resolution");
        _powerReceivedL2 = Metrics.CreateGauge("p1_power_received_l2", "Instantaneous active power received for phase L2 (-P) in W resolution");
        _powerReceivedL3 = Metrics.CreateGauge("p1_power_received_l3", "Instantaneous active power received for phase L3 (-P) in W resolution");
    }

    /// <summary>
    /// Set metrics for all P1 values.
    /// </summary>
    /// <param name="gauge">The gauge to set the value for.</param>
    /// <param name="value">The value to set.</param>
    private static void TrySet(Gauge gauge, double? value)
    {
        if (value != null)
            gauge.Set(value.Value);
    }


    /// <summary>
    /// Set metrics for all modbus devices.
    /// </summary>
    /// <param name="p1Message">The <see cref="P1Message"/> to get the modbus values from.</param>
    private void SetModbusMetrics(P1Message p1Message)
    {
        var index = 0;
        foreach (var mbus in p1Message.MBusClients)
        {
            //Get or create device identifier.
            var id = mbus.DeviceIdentifier ?? (index+1).ToString();
            index++;

            //Create new metrics if needed.
            if (!_modbusMetrics.TryGetValue(id, out var metrics))
            {
                _logger.LogInformation("Adding new modbus device {id}", id);
                metrics = new ModbusMetrics()
                {
                    ModbusDeviceType = Metrics.CreateGauge($"p1_mbus_{id}_device_type", $"Modbus client {id} device type"),
                    ModbusCaptureTime = Metrics.CreateGauge($"p1_mbus_{id}_capture_time",
                        $"Modbus client {id} metric capture time"),
                    ModbusValue = Metrics.CreateGauge($"p1_mbus_{id}_value", $"Modbus client {id} metric value")
                };
                _modbusMetrics.Add(id, metrics);
            }

            //Set values
            if (mbus.CaptureTime != null)
                metrics.ModbusCaptureTime.Set(mbus.CaptureTime.Value.ToFileTime());
            TrySet(metrics.ModbusDeviceType, mbus.DeviceType);
            TrySet(metrics.ModbusValue, mbus.Value);
        }
    }

    private class ModbusMetrics
    {
        public Gauge ModbusDeviceType;
        public Gauge ModbusCaptureTime;
        public Gauge ModbusValue;
    }
}