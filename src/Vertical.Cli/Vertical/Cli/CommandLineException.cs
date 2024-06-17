using Vertical.Cli.Configuration;

namespace Vertical.Cli;

/// <summary>
/// Represents an error during command invocation.
/// </summary>
public sealed class CommandLineException : Exception
{
    internal CommandLineException(
        string message,
        string path,
        CliSymbol? symbol = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Path = path;
        Symbol = symbol;
    }
    
    /// <summary>
    /// Gets the path.
    /// </summary>
    public string Path { get; }
    
    /// <summary>
    /// Gets the affected symbol, if available.
    /// </summary>
    public CliSymbol? Symbol { get; }
}