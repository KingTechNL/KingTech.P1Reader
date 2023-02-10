using Flurl.Http;
using Flurl.Http.Configuration;
using KingTech.P1Reader.Broker;
using KingTech.P1Reader.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace KingTech.P1Reader.Services;

/// <summary>
/// This service is used to publish incoming messages to the DSMR Reader.
/// This way the P1Reader can be used as a remote agent for the official DSMR reader software.
/// https://github.com/dsmrreader/dsmr-reader
/// </summary>
public class DsmrReaderPublisherService
{
    private readonly ILogger<DsmrReaderPublisherService> _logger;
    private readonly DsmrReaderPublisherSettings _settings;
    private readonly IMessageBroker<P1Message> _messageBroker;
    private readonly IFlurlClient _dsmrClient;
    
    private readonly CancellationTokenSource _cancellationTokenSource;

    public DsmrReaderPublisherService(ILogger<DsmrReaderPublisherService> logger, 
        DsmrReaderPublisherSettings settings, IMessageBroker<P1Message> messageBroker, 
        IFlurlClientFactory clientFactory)
    {
        _logger = logger;
        _settings = settings;
        _messageBroker = messageBroker;
        _dsmrClient = clientFactory.Get(settings.Host);
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        if (_settings?.Host == null || _settings?.ApiKey == null)
            return;

        _messageBroker.SubscribeASync(PushData);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _messageBroker.UnsubscribeASync(PushData);
    }

    private async Task PushData(P1Message? message)
    {
        //Check if publisher is ready to call DSMR reader.
        if(_cancellationTokenSource.IsCancellationRequested)
            return;

        //Verify data.
        if (message?.ElectricityUsageLow == null 
            || message?.ElectricityUsageHigh == null
            || message?.ElectricityReturnedLow == null 
            || message?.ElectricityReturnedHigh == null
            || message?.ActualElectricityDelivered == null 
            || message?.ActualElectricityRetrieved == null)
            return;

        //Create contract.
        var contract = new DsmrContract()
        {
            Timestamp = message.Timestamp ?? DateTime.Now,
            ElectricityDeliveredLow = message.ElectricityUsageLow?.ToString("0.###", CultureInfo.InvariantCulture)!,
            ElectricityReturnedLow = message.ElectricityReturnedLow?.ToString("0.###", CultureInfo.InvariantCulture)!,
            ElectricityDeliveredHigh = message.ElectricityUsageHigh?.ToString("0.###", CultureInfo.InvariantCulture)!,
            ElectricityReturnedHigh = message.ElectricityReturnedHigh?.ToString("0.###", CultureInfo.InvariantCulture)!,
            ElectricityCurrentlyDelivered = message.ActualElectricityDelivered?.ToString("0.###", CultureInfo.InvariantCulture)!,
            ElectricityCurrentlyReturned = message.ActualElectricityRetrieved?.ToString("0.###", CultureInfo.InvariantCulture)!,
            CurrentlyDeliveredL1 = message.PowerDeliveredL1?.ToString("0.###", CultureInfo.InvariantCulture),
            CurrentlyDeliveredL2 = message.PowerDeliveredL2?.ToString("0.###", CultureInfo.InvariantCulture),
            CurrentlyDeliveredL3 = message.PowerDeliveredL3?.ToString("0.###", CultureInfo.InvariantCulture),
            CurrentlyReturnedL1 = message.PowerReceivedL1?.ToString("0.###", CultureInfo.InvariantCulture),
            CurrentlyReturnedL2 = message.PowerReceivedL2?.ToString("0.###", CultureInfo.InvariantCulture),
            CurrentlyReturnedL3 = message.PowerReceivedL3?.ToString("0.###", CultureInfo.InvariantCulture),
            VoltageL1 = message.VoltageL1?.ToString("0.#", CultureInfo.InvariantCulture),
            VoltageL2 = message.VoltageL2?.ToString("0.#", CultureInfo.InvariantCulture),
            VoltageL3 = message.VoltageL3?.ToString("0.#", CultureInfo.InvariantCulture),
            CurrentL1 = message.CurrentL1,
            CurrentL2 = message.CurrentL2,
            CurrentL3 = message.CurrentL3,

            ExtraDeviceTimestamp = message.MBusClients?.FirstOrDefault()?.CaptureTime ??
                                   (message.MBusClients?.FirstOrDefault()?.Value != null ? DateTime.Now : null),
            ExtraDeviceDelivered = message.MBusClients?.FirstOrDefault()?.Value?.ToString("0.###", CultureInfo.InvariantCulture),
        };

        //Send data to DSMR Reader.
        var url = Flurl.Url.Combine(_settings.Host, "/api/v2/datalogger/dsmrreading");
        var json = JsonConvert.SerializeObject(contract, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });
        _logger.LogTrace("Sending {@Contract} to DSMR Reader on {url}", json, url.ToString());
        
        try
        {
            var apiResponse = await url
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Authorization", $"Token {_settings.ApiKey}")
                .WithClient(_dsmrClient)
                .AllowAnyHttpStatus()
                .WithAutoRedirect(false)
                .WithTimeout(_settings.Timeout)
                .PostJsonAsync(contract, _cancellationTokenSource.Token);

            _logger.LogTrace("Response from DSMR Reader: {@Response}",
                apiResponse.ResponseMessage.Content.ToString());

            if (apiResponse.StatusCode != 201)
            {
                var response = await apiResponse.GetJsonAsync<JObject>();
                _logger.LogError("Failed to push data {Contract} to DSMR Reader. Status code: {StatusCode}, Response {@Response}",
                    json, apiResponse.StatusCode, response);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to push {@Data} to DSMR Reader", contract);
        }
    }

    /// <summary>
    /// POST request body for DSMR reader API.
    /// </summary>
    private class DsmrContract
    {
        /// <summary>
        /// Timestamp for the measurement, according to the meter.
        /// Moment waarop de meting is gedaan, volgens de meter
        /// </summary>
        /// <example>2019-08-24T14:15:22Z</example>
        [JsonRequired]
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Measurement of used electricity (low tariff) in kWh.
        /// Meterstand van verbruikte elektriciteit (Nederlandse gebruikers: dal-/laagtarief) in kWh.
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonRequired]
        [JsonProperty("electricity_delivered_1")]
        public string ElectricityDeliveredLow { get; set; }
        /// <summary>
        /// Measurement of returned electicity (low tariff) in kWh.
        /// Meterstand van teruggeleverde elektriciteit (Nederlandse gebruikers: dal-/laagtarief) in kWh.
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonRequired]
        [JsonProperty("electricity_returned_1")]
        public string ElectricityReturnedLow { get; set; }
        /// <summary>
        /// Measurement of used electicity (high tariff) in kWh.
        /// Meterstand van verbruikte elektriciteit (piektarief) in kWh.
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonRequired]
        [JsonProperty("electricity_delivered_2")]
        public string ElectricityDeliveredHigh { get; set; }
        /// <summary>
        /// Measurement of returned electicity (high tariff) in kWh.
        /// Meterstand van teruggeleverde elektriciteit (piektarief) in kWh.
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonRequired]
        [JsonProperty("electricity_returned_2")]
        public string ElectricityReturnedHigh { get; set; }

        /// <summary>
        /// Current electricity usage in kW.
        /// Huidig elektriciteitsverbruik in kW.
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonRequired]
        [JsonProperty("electricity_currently_delivered")]
        public string ElectricityCurrentlyDelivered { get; set; }
        /// <summary>
        /// Current electricity returned in kW.
        /// Huidige elektriciteitsteruglevering in kW.
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonRequired]
        [JsonProperty("electricity_currently_returned")]
        public string ElectricityCurrentlyReturned { get; set; }

        /// <summary>
        /// Current electricity usage in phase L1 (in kW).
        /// Huidig elektriciteitsverbruik in fase L1 (in kW).
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("phase_currently_delivered_l1")]
        public string? CurrentlyDeliveredL1 { get; set; }
        /// <summary>
        /// Current electricity usage in phase L2 (in kW).
        /// Huidig elektriciteitsverbruik in fase L2 (in kW).
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("phase_currently_delivered_l2")]
        public string? CurrentlyDeliveredL2 { get; set; }
        /// <summary>
        /// Current electricity usage in phase L3 (in kW).
        /// Huidig elektriciteitsverbruik in fase L3 (in kW).
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("phase_currently_delivered_l3")]
        public string? CurrentlyDeliveredL3 { get; set; }

        /// <summary>
        /// Current electricity returned in phase L1 (in kW).
        /// Huidige elektriciteitsteruglevering in fase L1 (in kW).
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("phase_currently_returned_l1")]
        public string? CurrentlyReturnedL1 { get; set; }
        /// <summary>
        /// Current electricity returned in phase L2 (in kW).
        /// Huidige elektriciteitsteruglevering in fase L2 (in kW).
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("phase_currently_returned_l2")]
        public string? CurrentlyReturnedL2 { get; set; }
        /// <summary>
        /// Current electricity returned in phase L3 (in kW).
        /// Huidige elektriciteitsteruglevering in fase L3 (in kW).
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("phase_currently_returned_l3")]
        public string? CurrentlyReturnedL3 { get; set; }

        /// <summary>
        /// Current voltage in phase L1 (in V).
        /// Huidig voltage in fase L1 (in V).
        /// </summary>
        /// <example>decimal places <=1 [-1000..1000]</example>
        [JsonProperty("phase_voltage_l1")]
        public string? VoltageL1 { get; set; }
        /// <summary>
        /// Current voltage in phase L2 (in V).
        /// Huidig voltage in fase L2 (in V).
        /// </summary>
        /// <example>decimal places <=1 [-1000..1000]</example>
        [JsonProperty("phase_voltage_l2")]
        public string? VoltageL2 { get; set; }
        /// <summary>
        /// Current voltage in phase L3 (in V).
        /// Huidig voltage in fase L3 (in V).
        /// </summary>
        /// <example>decimal places <=1 [-1000..1000]</example>
        [JsonProperty("phase_voltage_l3")]
        public string? VoltageL3 { get; set; }

        /// <summary>
        /// Current power level in phase L1 (in A).
        /// Huidige stroomsterkte in fase L1 (in A).
        /// </summary>
        /// <example>[0..999]</example>
        [JsonProperty("phase_power_current_l1")]
        public long? CurrentL1 { get; set; }
        /// <summary>
        /// Current power level in phase L2 (in A).
        /// Huidige stroomsterkte in fase L2 (in A).
        /// </summary>
        /// <example>[0..999]</example>
        [JsonProperty("phase_power_current_l2")]
        public long? CurrentL2 { get; set; }
        /// <summary>
        /// Current power level in phase L3 (in A).
        /// Huidige stroomsterkte in fase L3 (in A).
        /// </summary>
        /// <example>[0..999]</example>
        [JsonProperty("phase_power_current_l3")]
        public long? CurrentL3 { get; set; }

        /// <summary>
        /// Timestamp for the measurement of the external (gas) meter.
        /// Tijdstip van meting door de externe (gas)meter.
        /// </summary>
        /// <example>2019-08-24T14:15:22Z</example>
        [JsonProperty("extra_device_timestamp")]
        public DateTime? ExtraDeviceTimestamp { get; set; }
        /// <summary>
        /// Measurement of the external (gas) meter.
        /// Meterstand van de externe (gas)meter
        /// </summary>
        /// <example>decimal places <=3 [-1000000..1000000]</example>
        [JsonProperty("extra_device_delivered")]
        public string? ExtraDeviceDelivered { get; set; }
    }
}

public class DsmrReaderPublisherSettings
{
    public string? Host { get; set; }
    public string? ApiKey { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(3);
}