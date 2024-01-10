using System.Text.RegularExpressions;

namespace Vertical.Cli.Utilities;

#if NET7_0_OR_GREATER    
internal static partial class MessageFormatter
#else
internal static class MessageFormatter
#endif
{
    internal static string GetString(string format, IDictionary<string, object> arguments)
    {
        return MyRegex().Replace(format, match =>
        {
            var key = match.Groups[1].Value;
            if (!arguments.TryGetValue(key, out var obj))
                return $"{{{key}}}";

            return obj?.ToString() ?? "(null)";
        });
    }

#if NET7_0_OR_GREATER    
    [GeneratedRegex(@"(?<!\{)\{((?:\{\{)*\w+(?:\}\})*)\}(?!\})")]
    private static partial Regex MyRegex();
#else
    private static Regex MyRegex() => new(@"(?<!\{)\{((?:\{\{)*\w+(?:\}\})*)\}(?!\})");
#endif

}