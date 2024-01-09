using Vertical.Cli.Configuration;

namespace Vertical.Cli;

/// <summary>
/// Indicates an operand was not provided for an option.
/// </summary>
public sealed class CliMissingOperandException : CliArgumentException
{
    /// <inheritdoc />
    internal CliMissingOperandException(
        string format,
        IDictionary<string, object> arguments,
        SymbolDefinition symbol,
        Exception? innerException = null) 
        : base(format, arguments, innerException)
    {
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the symbol that could not be bound.
    /// </summary>
    public SymbolDefinition Symbol { get; }
}