namespace KingTech.P1Reader.Parser;

/// <summary>
/// Parser for telegrams containing P1 messages.
/// </summary>
internal interface IP1Parser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="telegram"></param>
    /// <returns></returns>
    P1Message? ParseTelegram(string telegram);
}