using System.Text;

namespace KingTech.P1Reader.NuGet.Parser;

/// <summary>
/// Base class for parsing P1 messages.
/// </summary>
internal abstract class ABaseP1Parser : IP1Parser
{
    /// <summary>
    /// Parse a telegram into a P1Message.
    /// </summary>
    /// <param name="telegram">The Telegram to parse.</param>
    /// <returns>The resulting P1 message.</returns>
    public abstract P1Message? ParseTelegram(string telegram);


    /// <summary>
    /// Parse the telegram string into key/value pairs representing the P1 values.
    /// </summary>
    /// <param name="telegram">The telegram message to parse.</param>
    /// <returns>Dictionary containing all P1 values.</returns>
    protected Dictionary<string, List<string>> ParseTelegramValues(string telegram)
    {
        //var _regex = new Regex(@"(.*)(\((.*?)\))");
        //var matches = _regex.Matches(telegram);
        //return matches.Where(match => match.Success && match.Groups.Count > 2)
        //    .Select(match => new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value))
        //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var result = new Dictionary<string, List<string>>();
        var values = telegram.Split('\n').Select(SplitTelegramLine);
        foreach (var value in values)
            if (value.Count() > 1)
                result.Add(value[0], value.GetRange(1, value.Count - 1));
        return result;
    }

    /// <summary>
    /// Split a single line of the telegram message into its components.
    /// A typical line looks something like this:
    ///    0-2:24.2.1(230205164505W)(06919.956*m3)
    /// </summary>
    /// <param name="line">The line of the telegram to split.</param>
    /// <returns>A list containing all components of the telegram line.</returns>
    protected List<string> SplitTelegramLine(string line)
    {
        var result = new List<string>();
        var builder = new StringBuilder();
        foreach (var character in line)
        {

            //Get first part of line.
            if (result.Count < 1)
            {
                if (character == '(')
                {
                    result.Add(builder.ToString());
                    builder.Clear();
                    continue;
                }
                builder.Append(character);
                continue;
            }

            //Ignore (
            if (character == '(')
                continue;

            //End of value
            if (character == ')')
            {
                result.Add(builder.ToString());
                builder.Clear();
                continue;
            }
            builder.Append(character);
        }

        return result;
    }

    /// <summary>
    /// Parse a long value from the given dictionary.
    /// </summary>
    /// <param name="dict">The dictionary to parse the long from.</param>
    /// <param name="key">The key to find the value to parse in the dictionary.</param>
    /// <param name="index">The index of the value to parse in the list value.</param>
    /// <returns></returns>
    protected long ParseLong(Dictionary<string, List<string>> dict, string key, int index = 0)
    {
        if (!dict.TryGetValue(key, out var value))
            return 0;

        var correctedString = TrimValue(value[index]);
        if (long.TryParse(correctedString, out var result))
            return result;
        return 0;
    }

    /// <summary>
    /// Parse a double value from the given dictionary.
    /// </summary>
    /// <param name="dict">The dictionary to parse the long from.</param>
    /// <param name="key">The key to find the value to parse in the dictionary.</param>
    /// <param name="index">The index of the value to parse in the list value.</param>
    protected double ParseDouble(Dictionary<string, List<string>> dict, string key, int index = 0)
    {
        if (!dict.TryGetValue(key, out var value))
            return 0;

        var correctedString = TrimValue(value[index]);
        if (double.TryParse(correctedString, out var result))
            return result;
        return 0;
    }

    /// <summary>
    /// Parse a timestamp value from the given dictionary.
    /// </summary>
    /// <param name="dict">The dictionary to parse the long from.</param>
    /// <param name="key">The key to find the value to parse in the dictionary.</param>
    /// <param name="index">The index of the value to parse in the list value.</param>
    protected long ParseTimestamp(Dictionary<string, List<string>> dict, string key, int index = 0)
    {
        if (!dict.TryGetValue(key, out var value))
            return 0;

        var correctedString = TrimValue(value[index]);
        if (long.TryParse(correctedString, out var result))
            return result;
        return 0;
    }

    /// <summary>
    /// Trim all non-numeric characters from a string.
    /// </summary>
    /// <param name="value">the string to trim.</param>
    /// <returns>The string without its numeric characters.</returns>
    protected string TrimValue(string value)
    {
        //This is faster then using regex.
        var trimmedValue = new string(value.Where(c => c == '-' || c == '.' || (c >= '0' && c <= '9')).ToArray());
        return trimmedValue.Replace('.', ',');
    }
}