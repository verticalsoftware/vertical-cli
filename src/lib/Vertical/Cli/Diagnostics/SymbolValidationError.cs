using Vertical.Cli.Configuration;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates application input failed validation.
/// </summary>
public sealed class SymbolValidationError : CommandLineError
{
    internal SymbolValidationError(
        CliSymbol symbol,
        object model,
        object? receivedValue,
        string validationMessage) : base(FormatMessage(symbol, validationMessage))
    {
        Symbol = symbol;
        Model = model;
        ReceivedValue = receivedValue;
    }

    /// <summary>
    /// Gets the associated symbol.
    /// </summary>
    public CliSymbol Symbol { get; }
    
    /// <summary>
    /// Gets a reference to the model.
    /// </summary>
    public object Model { get; }

    /// <summary>
    /// Gets the received value.
    /// </summary>
    public object? ReceivedValue { get; }

    internal static SymbolValidationError Create<TModel, TValue>(
        ValidationEventInfo<TModel, TValue> eventInfo,
        string message)
        where TModel : class
    {
        return new SymbolValidationError(
            eventInfo.Symbol,
            eventInfo.Model,
            eventInfo.Value,
            message);
    }

    private static string FormatMessage(CliSymbol symbol, string message)
    {
        var identifier = GetSymbolIdentifier(symbol);
        return $"{identifier}: {message}";
    }
}