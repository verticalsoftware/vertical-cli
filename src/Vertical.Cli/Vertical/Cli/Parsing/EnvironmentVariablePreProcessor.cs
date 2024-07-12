using System.Text.RegularExpressions;

namespace Vertical.Cli.Parsing;

internal static partial class EnvironmentVariablePreProcessor
{
    internal static void Handle(LinkedList<string> argumentList)
    {
        var platform = Environment.OSVersion.Platform;
        
        argumentList.EvaluateEach(value =>
        {
            return platform switch
            {
                PlatformID.Unix => TryReplaceEnvironmentVariableSymbols(UnixRegex(), value),
                PlatformID.MacOSX => TryReplaceEnvironmentVariableSymbols(UnixRegex(), value),
                PlatformID.Win32NT => TryReplaceEnvironmentVariableSymbols(WindowsRegex(), value),
                _ => value
            };
        });
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
    private static partial Regex UnixRegex();
    
    [GeneratedRegex(@"\$env:(\w[\w\d_()]+)")]
    private static partial Regex WindowsRegex();
}