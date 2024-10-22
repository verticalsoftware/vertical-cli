using System.Text.RegularExpressions;

namespace Vertical.Cli.Parsing;

internal static partial class EnvironmentVariablePreProcessor
{
    internal static void Handle(LinkedList<string> argumentList)
    {
        argumentList.EvaluateEach(value => TryReplaceEnvironmentVariableSymbols(SymbolRegex(), value));
    }

    private static string TryReplaceEnvironmentVariableSymbols(Regex regex, string value)
    {
        return regex.Replace(value, match =>
        {
            var variableName = match.Groups[1].Value;
            var replacementValue = Environment.GetEnvironmentVariable(variableName);
            return replacementValue ?? string.Empty;
        });
    }

    [GeneratedRegex(@"\$([A-Z][A-Z\d_]+)")]
    private static partial Regex SymbolRegex();
}