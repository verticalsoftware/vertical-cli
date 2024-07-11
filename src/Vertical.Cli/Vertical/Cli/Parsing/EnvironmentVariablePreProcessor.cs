using System.Text.RegularExpressions;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines a pre-processor that replaces tokens in arguments using environment variable
/// values.
/// </summary>
public static partial class EnvironmentVariablePreProcessor
{
    /// <summary>
    /// Replaces tokens in the arguments with environment variable values.
    /// </summary>
    /// <param name="argumentList">Argument list</param>
    public static void Handle(LinkedList<string> argumentList)
    {
        var platform = Environment.OSVersion.Platform;
        
        for (var node = argumentList.First; node != null; node = node.Next)
        {
            var argument = node.Value;
            var newValue = platform switch
            {
                PlatformID.Unix => TryReplaceEnvironmentVariableSymbols(UnixRegex(), argument),
                PlatformID.MacOSX => TryReplaceEnvironmentVariableSymbols(UnixRegex(), argument),
                PlatformID.Win32NT => TryReplaceEnvironmentVariableSymbols(WindowsRegex(), argument),
                _ => node.Value
            };

            if (newValue.Equals(node.Value))
                continue;

            var thisNode = node;
            node = argumentList.AddAfter(thisNode, newValue);
            argumentList.Remove(thisNode);
        }
    }
    
    /// <summary>
    /// Replaces tokens in the arguments with environment variable values and invokes the next
    /// processor.
    /// </summary>
    /// <param name="argumentList">Argument list</param>
    /// <param name="next">Action that invokes the next pre-processor</param>
    public static void Handle(LinkedList<string> argumentList, Action<LinkedList<string>> next)
    {
        Handle(argumentList);
        next(argumentList);
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

    [GeneratedRegex(@"\$([^\s]+)")]
    private static partial Regex UnixRegex();
    
    [GeneratedRegex(@"\$env:([^\s]+)")]
    private static partial Regex WindowsRegex();
}