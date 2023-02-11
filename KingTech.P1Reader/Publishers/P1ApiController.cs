using System.Net.Mime;
using KingTech.P1Reader.Message;
using KingTech.P1Reader.Model;
using Microsoft.AspNetCore.Mvc;

namespace KingTech.P1Reader.Publishers;

/// <summary>
/// Main controller for the P1 reader.
/// Enables basic API functionality.
/// </summary>
[ApiController]
[Route("api")]
public class P1ApiController : ControllerBase
{

    private readonly ILogger<P1ApiController> _logger;
    private readonly IP1Receiver _receiver;

    /// <summary>
    /// Main controller for the P1 reader.
    /// Enables basic API functionality.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to log messages from this controller.</param>
    /// <param name="receiver">The <see cref="IP1Receiver"/> to check the last received telegram message.</param>
    public P1ApiController(ILogger<P1ApiController> logger, IP1Receiver receiver)
    {
        _logger = logger;
        _receiver = receiver;
    }

    /// <summary>
    /// Get the received P1 message in json format.
    /// </summary>
    /// <returns>The received P1 message in json format.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(type: typeof(List<P1Contract>), statusCode: StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(ConvertMessageToContract(_receiver?.LastTelegram));
    }

    /// <summary>
    /// Get the raw p1 message as it was received from the energy meter.
    /// </summary>
    /// <returns>The raw p1 message as it was received from the energy meter.</returns>
    [HttpGet("raw")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status200OK)]
    public IActionResult GetRaw()
    {
        return Ok(_receiver.LastTelegram?.RawMessage);
    }

    /// <summary>
    /// Convert the received <see cref="P1Message"/> to a <see cref="P1Contract"/>.
    /// </summary>
    /// <param name="message">The <see cref="P1Message"/> to convert.</param>
    /// <returns>The resulting <see cref="P1Contract"/>.</returns>
    private P1Contract ConvertMessageToContract(P1Message? message) =>
        new()
        {
            EquipmentIdentifier = message?.EquipmentIdentifier,
            VersionInformation = message?.VersionInformation,
            Timestamp = message?.Timestamp,
            RawTimestamp = message?.RawTimestamp,
            ElectricityUsageHigh = message?.ElectricityUsageHigh,
            ElectricityUsageLow = message?.ElectricityUsageLow,
            ElectricityReturnedHigh = message?.ElectricityReturnedHigh,
            ElectricityReturnedLow = message?.ElectricityReturnedLow,
            TariffIndicator = message?.TariffIndicator,
            PowerFailuresLong = message?.PowerFailuresLong,
            PowerFailuresShort = message?.PowerFailuresShort,
            ActualElectricityDelivered = message?.ActualElectricityDelivered,
            ActualElectricityRetrieved = message?.ActualElectricityRetrieved,
            PowerFailureEventLog = message?.PowerFailureEventLog,
            TextMessage = message?.TextMessage,

            //Get PhaseL1 data.
            PhaseL1 = message?.VoltageSagsL1 != null || message?.VoltageSwellsL1 != null ||
                       message?.CurrentL1 != null || message?.VoltageL1 != null ||
                       message?.PowerDeliveredL1 != null || message?.PowerReceivedL1 != null
                ? new PhaseDataModel()
                {
                    VoltageSags = message?.VoltageSagsL1,
                    VoltageSwells = message?.VoltageSwellsL1,
                    Current = message?.CurrentL1,
                    Voltage = message?.VoltageL1,
                    PowerDelivered = message?.PowerDeliveredL1,
                    PowerReceived = message?.PowerReceivedL1,
                }
                : null,
            //Get PhaseL2 data.
            PhaseL2 = message?.VoltageSagsL2 != null || message?.VoltageSwellsL2 != null ||
                       message?.CurrentL2 != null || message?.VoltageL2 != null ||
                       message?.PowerDeliveredL2 != null || message?.PowerReceivedL2 != null
                ? new PhaseDataModel()
                {
                    VoltageSags = message?.VoltageSagsL2,
                    VoltageSwells = message?.VoltageSwellsL2,
                    Current = message?.CurrentL2,
                    Voltage = message?.VoltageL2,
                    PowerDelivered = message?.PowerDeliveredL2,
                    PowerReceived = message?.PowerReceivedL2,
                }
                : null,
            //Get PhaseL3 data.
            PhaseL3 = message?.VoltageSagsL3 != null || message?.VoltageSwellsL3 != null ||
                       message?.CurrentL3 != null || message?.VoltageL3 != null ||
                       message?.PowerDeliveredL3 != null || message?.PowerReceivedL3 != null
                ? new PhaseDataModel()
                {
                    VoltageSags = message?.VoltageSagsL3,
                    VoltageSwells = message?.VoltageSwellsL3,
                    Current = message?.CurrentL3,
                    Voltage = message?.VoltageL3,
                    PowerDelivered = message?.PowerDeliveredL3,
                    PowerReceived = message?.PowerReceivedL3,
                }
                : null,

            ModBusClients = message?.MBusClients?.Select(ConvertMBusClientToModbusClientModel)
        };

    /// <summary>
    /// Convert a <see cref="MBusClient"/> to a <see cref="ModbusClientModel"/>.
    /// </summary>
    /// <param name="model">The <see cref="MBusClient"/> to convert.</param>
    /// <returns>The resulting <see cref="ModbusClientModel"/></returns>
    private ModbusClientModel ConvertMBusClientToModbusClientModel(MBusClient? model) =>
        new()
        {
            DeviceIdentifier = model.DeviceIdentifier,
            DeviceType = model.DeviceType,
            CaptureTime = model.CaptureTime,
            RawCaptureTime = model.RawCaptureTime,
            Value = model.Value
        };
}