using Vertical.Cli.Utilities;

namespace Vertical.Cli;

/// <summary>
/// Indicates one or more command line arguments were invalid.
/// </summary>
public abstract class CliArgumentException : Exception
{
    internal CliArgumentException(
        string format,
        IDictionary<string, object> arguments,
        Exception? innerException = null)
        : base(MessageFormatter.GetString(format, arguments), innerException)
    {
        OriginalFormat = format;
        Arguments = arguments;
    }

    /// <summary>
    /// Gets the original message format.
    /// </summary>
    public string OriginalFormat { get; set; }

    /// <summary>
    /// Gets the formatting arguments.
    /// </summary>
    public IDictionary<string, object> Arguments { get; }
}