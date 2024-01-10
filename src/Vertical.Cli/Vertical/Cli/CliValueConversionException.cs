using Vertical.Cli.Configuration;

namespace Vertical.Cli;

public sealed class CliValueConversionException : CliArgumentException
{
    /// <inheritdoc />
    internal CliValueConversionException(
        string format,
        IDictionary<string, object> arguments,
        SymbolDefinition symbol,
        string attemptedValue,
        Exception? innerException = null) 
        : base(format, arguments, innerException)
    {
        Symbol = symbol;
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Gets the symbol that could not be bound.
    /// </summary>
    public SymbolDefinition Symbol { get; }

    /// <summary>
    /// Gets the attempted value.
    /// </summary>
    public string AttemptedValue { get; }
}