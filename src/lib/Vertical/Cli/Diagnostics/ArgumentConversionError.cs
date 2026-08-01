using Vertical.Cli.Configuration;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates a string argument could not be converted to another property type.
/// </summary>
public class ArgumentConversionError : CommandLineError
{
    internal ArgumentConversionError(
        ICliSymbol symbol,
        Type expectedType,
        string receivedArgument,
        Exception? exception = null)
        : base(FormatMessage(symbol, expectedType, receivedArgument))
    {
        Symbol = symbol;
        ExpectedType = expectedType;
        ReceivedArgument = receivedArgument;
        Exception = exception;
    }

    /// <summary>
    /// Gets the affected symbol.
    /// </summary>
    public ICliSymbol Symbol { get; }

    /// <summary>
    /// Gets the type the argument should be convertible to.
    /// </summary>
    public Type ExpectedType { get; }

    /// <summary>
    /// Gets the argument that was received.
    /// </summary>
    public string ReceivedArgument { get; }
    
    /// <summary>
    /// Gets the exception (if available) that occurred during the conversion.
    /// </summary>
    public Exception? Exception { get; }

    private static string FormatMessage(ICliSymbol symbol, Type expectedType, string receivedArgument)
    {
        var identifier = GetSymbolIdentifier(symbol);

        return $"{identifier}: cannot convert '{receivedArgument}' to {expectedType.Name}.";
    }
}