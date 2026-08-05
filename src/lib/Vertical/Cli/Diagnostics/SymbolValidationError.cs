using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates application input failed validation.
/// </summary>
public sealed class SymbolValidationError : CommandLineError
{
    private SymbolValidationError(
        CliSymbol symbol,
        object model,
        object? receivedValue,
        string validationMessage,
        string message) : base(message)
    {
        Symbol = symbol;
        Model = model;
        ReceivedValue = receivedValue;
        ValidationMessage = validationMessage;
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

    /// <summary>
    /// Gets the validation message.
    /// </summary>
    public string ValidationMessage { get; }

    internal static SymbolValidationError Create<TModel, TValue>(
        ValidationEventInfo<TModel, TValue> eventInfo,
        string validationMessage,
        IHelpProvider helpProvider)
        where TModel : class
    {
        var identifier = GetSymbolIdentifier(helpProvider, eventInfo.Symbol);
        var baseMessage = $"{identifier}: {validationMessage}";
        
        return new SymbolValidationError(
            eventInfo.Symbol,
            eventInfo.Model,
            eventInfo.Value,
            validationMessage, 
            baseMessage);
    }
}