using System.Text.RegularExpressions;

namespace Vertical.Cli.Utilities;

internal static class MessageFormatter
{
    internal static string GetString(string format, IDictionary<string, object> arguments)
    {
        return Regex.Replace(format, @"(?<!\{)\{((?:\{\{)*\w+(?:\}\})*)\}(?!\})", match =>
        {
            var key = match.Groups[1].Value;
            if (!arguments.TryGetValue(key, out var obj))
                return $"{{{key}}}";

            return obj?.ToString() ?? "(null)";
        });
    }
}