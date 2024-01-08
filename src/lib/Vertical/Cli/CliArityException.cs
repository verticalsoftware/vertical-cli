using Vertical.Cli.Configuration;

namespace Vertical.Cli;

public sealed class CliArityException : CliArgumentException
{
    /// <inheritdoc />
    internal CliArityException(
        string format, 
        IDictionary<string, object> arguments,
        SymbolDefinition symbol,
        int receivedCount,
        Exception? innerException = null) :
        base(format, arguments, innerException)
    {
        Symbol = symbol;
        ReceivedCount = receivedCount;
    }

    /// <summary>
    /// Gets the symbol that could not be bound.
    /// </summary>
    public SymbolDefinition Symbol { get; }

    /// <summary>
    /// Gets the actual number of arguments received.
    /// </summary>    
    public int ReceivedCount { get; }
}