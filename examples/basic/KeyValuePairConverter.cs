using System.Text.RegularExpressions;

namespace BasicDemo;

public static partial class KeyValuePairConverter
{
    public static readonly Converter<string, KeyValuePair<string, string>> Instance =
        str => MyRegex().Match(str) is { Success: true } match
            ? new KeyValuePair<string, string>(match.Groups["k"].Value, match.Groups["v"].Value)
            : throw new ArgumentException("Invalid metadata value.");

    [GeneratedRegex(@"(?<k>\w+):(?<v>.+)")]
    private static partial Regex MyRegex();
}