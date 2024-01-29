namespace Vertical.Cli;

/// <summary>
/// Indicates an error with a response file.
/// </summary>
public sealed class CliResponseFileException : CliArgumentException
{
    /// <inheritdoc />
    internal CliResponseFileException(
        string format,
        IDictionary<string, object> arguments,
        Exception? innerException = null) : base(format, arguments, innerException)
    {
    }
}