using System.Text.RegularExpressions;

namespace Vertical.Cli.ScenarioTests.Common;

public static partial class KeyValuePairConverter
{
    public static KeyValuePair<string, string> Convert(string str)
    {
        return MyRegex().Match(str) is { Success: true } match
            ? new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value)
            : throw new ArgumentException("invalid key/value pair format");
    }

    [GeneratedRegex(@"(?<key>\w+)[:=](?<value>.+)")]
    private static partial Regex MyRegex();
}