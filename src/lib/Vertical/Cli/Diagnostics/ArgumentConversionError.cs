using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates a string argument could not be converted to another property type.
/// </summary>
public class ArgumentConversionError : CommandLineError
{
    private ArgumentConversionError(
        ICliSymbol symbol,
        Type expectedType,
        string receivedArgument,
        string message,
        Exception? exception)
        : base(message)
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

    internal static ArgumentConversionError Create(
        ICliSymbol symbol,
        Type expectedType,
        string receivedArgument,
        IHelpProvider helpProvider,
        Exception? exception = null)
    {
        var identifier = GetSymbolIdentifier(helpProvider, symbol);
        var message = $"{identifier}: cannot convert '{receivedArgument}' to {expectedType.Name}.";

        
        return new ArgumentConversionError(
            symbol, 
            expectedType, 
            receivedArgument,
            message, 
            exception);
    }
}