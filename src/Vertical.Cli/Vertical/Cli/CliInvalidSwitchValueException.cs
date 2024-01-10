using Vertical.Cli.Configuration;

namespace Vertical.Cli;

/// <summary>
/// Indicates an argument was other than a boolean value.
/// </summary>
public sealed class CliInvalidSwitchValueException : CliArgumentException
{
    /// <inheritdoc />
    internal CliInvalidSwitchValueException(
        string format,
        IDictionary<string, object> arguments,
        SymbolDefinition symbol,
        string attemptedValue,
        Exception? innerException = null) : base(format, arguments, innerException)
    {
        Symbol = symbol;
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Gets the switch symbol that couldn't be bound.
    /// </summary>
    public SymbolDefinition Symbol { get; }

    /// <summary>
    /// Gets the attempted value.
    /// </summary>
    public string AttemptedValue { get; }
}