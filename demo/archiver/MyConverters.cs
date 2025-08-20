using System.Text.RegularExpressions;

namespace Vertical.Archiver;

public static class MyConverters
{
    public static TimeSpan? ToTimeSpan(string str)
    {
        if (Regex.Match(str, @"([0-9]+)([hms])") is not { Success: true } match)
            throw new FormatException("invalid time span format");

        var value = int.Parse(match.Groups[1].Value);

        return match.Groups[2].Value switch
        {
            "h" => TimeSpan.FromHours(value),
            "m" => TimeSpan.FromMinutes(value),
            _ => TimeSpan.FromSeconds(value)
        };
    }
}