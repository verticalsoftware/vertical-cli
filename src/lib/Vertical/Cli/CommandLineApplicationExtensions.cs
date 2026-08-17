using System.Text.RegularExpressions;
using Vertical.Cli.Conversion;

namespace Vertical.Cli;

/// <summary>
/// Defines ancillary configuration methods.
/// </summary>
public static partial class CommandLineApplicationExtensions
{
    /// <summary>
    /// Adds a conversion convention for string dictionaries.
    /// </summary>
    /// <param name="application">The application instance.</param>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// This method adds to converters:
    /// - An argument converter from string -> KeyValuePair&lt;string, string&gt;.
    /// - A collection converter from KeyValuePair&lt;string, string&gt; -> Dictionary&lt;string, string&gt;.
    /// Input arguments are matched the following pattern: &lt;key&gt;:value.
    /// </remarks>
    public static CommandLineApplication AddDictionaryConverter(this CommandLineApplication application)
    {
        application.AddArgumentConverter(arg => 
            KeyValuePairPattern().Match(arg) is not { Success: true } match ? 
                throw new ArgumentException("invalid key/value pair syntax.") 
                : new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value));
        
        application.AddCollectionConverter(
            (IEnumerable<KeyValuePair<string, string>> values) =>
                new Dictionary<string, string>(values));

        return application;
    }

    [GeneratedRegex(@"(?<key>\w+):(?<value>.+)")]
    private static partial Regex KeyValuePairPattern();
}