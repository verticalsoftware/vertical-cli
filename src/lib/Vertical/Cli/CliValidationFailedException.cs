using Vertical.Cli.Configuration;

namespace Vertical.Cli;

public sealed class CliValidationFailedException : CliArgumentException
{
    /// <inheritdoc />
    internal CliValidationFailedException(
        string format,
        IDictionary<string, object> arguments,
        SymbolDefinition symbol,
        object? attemptedValue,
        string[] errors,
        Exception? innerException = null) 
        : base(format, arguments, innerException)
    {
        Symbol = symbol;
        AttemptedValue = attemptedValue;
        Errors = errors;
    }

    /// <summary>
    /// Gets the symbol that could not be bound.
    /// </summary>
    public SymbolDefinition Symbol { get; }

    /// <summary>
    /// Gets the attempted value.
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Gets the errors that were collected.
    /// </summary>
    public string[] Errors { get; }
}