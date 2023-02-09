using Flurl.Http;
using Flurl.Http.Configuration;
using KingTech.P1Reader.Message;
using Newtonsoft.Json;

namespace KingTech.P1Reader.Services;

/// <summary>
/// This service is used to publish incoming messages to the DSMR Reader.
/// This way the P1Reader can be used as a remote agent for the official DSMR reader software.
/// https://github.com/dsmrreader/dsmr-reader
/// </summary>
public class DsmrReaderPublisherService
{
    private const string PostUrl = "/api/v2/datalogger/dsmrreading";

    private readonly ILogger<DsmrReaderPublisherService> _logger;
    private readonly DsmrReaderPublisherSettings _settings;
    private readonly IP1Receiver _p1Receiver;
    private readonly IFlurlClient _dsmrClient;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public DsmrReaderPublisherService(ILogger<DsmrReaderPublisherService> logger, DsmrReaderPublisherSettings settings, IP1Receiver p1Receiver, IFlurlClientFactory clientFactory)
    {
        _logger = logger;
        _settings = settings;
        _p1Receiver = p1Receiver;
        _dsmrClient = clientFactory.Get(settings.Host);
        _cancellationTokenSource = new CancellationTokenSource(settings.Timeout);
    }

    public void Start()
    {
        if (_settings?.Host == null || _settings?.ApiKey == null)
            return;
        _p1Receiver.OnTelegramReceived += PushData;
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _p1Receiver.OnTelegramReceived -= PushData;
    }

    private async Task PushData(P1Message message)
    {
        if(message == null || _cancellationTokenSource.IsCancellationRequested)
            return;

        var contract = new DsmrContract(message);

        try
        {
            var url = Flurl.Url.Combine(_settings.Host, PostUrl);
            var json = JsonConvert.SerializeObject(contract);
            _logger.LogTrace("Sending {@Contract} to DSMR Reader on {url}", json, url.ToString());
            
            var apiResponse = await url
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Authorization", $"Token {_settings.ApiKey}")
                .WithClient(_dsmrClient)
                .PostJsonAsync(contract, _cancellationTokenSource.Token);

            if (apiResponse.StatusCode != 201)
            {
                string? response = (await apiResponse.GetJsonAsync())?.ToString();
                _logger.LogError("Failed to push data to DSMR Reader. Status code: {StatusCode}, Response {@Response}",
                    apiResponse.StatusCode, response);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to push {@Data} to DSMR Reader", contract);
        }
    }

    private class DsmrContract
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonProperty("electricity_delivered_1")]
        public string ElectricityDeliveredLow { get; set; }
        [JsonProperty("electricity_returned_1")]
        public string ElectricityReturnedLow { get; set; }
        [JsonProperty("electricity_delivered_2")]
        public string ElectricityDeliveredHigh { get; set; }
        [JsonProperty("electricity_returned_2")]
        public string ElectricityReturnedHigh { get; set; }

        [JsonProperty("electricity_currently_delivered")]
        public string ElectricityCurrentlyDelivered { get; set; }
        [JsonProperty("electricity_currently_returned")]
        public string ElectricityCurrentlyReturned { get; set; }
        
        [JsonProperty("phase_currently_delivered_l1")]
        public string? CurrentlyDeliveredL1 { get; set; }
        [JsonProperty("phase_currently_delivered_l2")]
        public string? CurrentlyDeliveredL2 { get; set; }
        [JsonProperty("phase_currently_delivered_l3")]
        public string? CurrentlyDeliveredL3 { get; set; }
        
        [JsonProperty("phase_currently_returned_l1")]
        public string? CurrentlyReturnedL1 { get; set; }
        [JsonProperty("phase_currently_returned_l2")]
        public string? CurrentlyReturnedL2 { get; set; }
        [JsonProperty("phase_currently_returned_l3")]
        public string? CurrentlyReturnedL3 { get; set; }
        
        [JsonProperty("phase_voltage_l1")]
        public string? VoltageL1 { get; set; }
        [JsonProperty("phase_voltage_l2")]
        public string? VoltageL2 { get; set; }
        [JsonProperty("phase_voltage_l3")]
        public string? VoltageL3 { get; set; }
        
        [JsonProperty("phase_power_current_l1")]
        public long? CurrentL1 { get; set; }
        [JsonProperty("phase_power_current_l2")]
        public long? CurrentL2 { get; set; }
        [JsonProperty("phase_power_current_l3")]
        public long? CurrentL3 { get; set; }

        [JsonProperty("extra_device_timestamp")]
        public DateTime? ExtraDeviceTimestamp { get; set; }
        [JsonProperty("extra_device_delivered")]
        public string? ExtraDeviceDelivered { get; set; }

        public DsmrContract(P1Message message)
        {
            Timestamp = message.Timestamp ?? DateTime.Now;
            ElectricityDeliveredLow = ToDsmrValue(message.ElectricityUsageLow);
            ElectricityReturnedLow = ToDsmrValue(message.ElectricityReturnedLow);
            ElectricityDeliveredHigh = ToDsmrValue(message.ElectricityUsageHigh);
            ElectricityReturnedHigh = ToDsmrValue(message.ElectricityReturnedHigh);
            ElectricityCurrentlyDelivered = ToDsmrValue(message.ActualElectricityDelivered);
            ElectricityCurrentlyReturned = ToDsmrValue(message.ActualElectricityRetrieved);
            CurrentlyDeliveredL1 = ToDsmrValue(message.PowerDeliveredL1);
            CurrentlyDeliveredL2 = ToDsmrValue(message.PowerDeliveredL2);
            CurrentlyDeliveredL3 = ToDsmrValue(message.PowerDeliveredL3);
            CurrentlyReturnedL1 = ToDsmrValue(message.PowerReceivedL1);
            CurrentlyReturnedL2 = ToDsmrValue(message.PowerReceivedL2);
            CurrentlyReturnedL3 = ToDsmrValue(message.PowerReceivedL3);
            VoltageL1 = ToDsmrValue(message.VoltageL1);
            VoltageL2 = ToDsmrValue(message.VoltageL2);
            VoltageL3 = ToDsmrValue(message.VoltageL3);
            CurrentL1 = message.CurrentL1 ?? 0;
            CurrentL2 = message.CurrentL2 ?? 0;
            CurrentL3 = message.CurrentL3 ?? 0;

            ExtraDeviceTimestamp = message.MBusClients?.FirstOrDefault()?.CaptureTime ??
                                   (message.MBusClients?.FirstOrDefault()?.Value != null ? DateTime.Now : null);
            ExtraDeviceDelivered = ToDsmrValue(message.MBusClients?.FirstOrDefault()?.Value);
        }
        
        private string ToDsmrValue(double? d)
        {
            int i = ((int)(d ?? 0));
            return i.ToString();
        }
    }
}

public class DsmrReaderPublisherSettings
{
    public string? Host { get; set; }
    public string? ApiKey { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(3);
}