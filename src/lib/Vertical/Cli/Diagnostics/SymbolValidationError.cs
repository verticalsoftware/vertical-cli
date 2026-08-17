using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates application input failed validation.
/// </summary>
public sealed class SymbolValidationError : CommandLineError
{
    private SymbolValidationError(
        IValidatable subject,
        object model,
        object? receivedValue,
        string validationMessage,
        string message) : base(message)
    {
        Subject = subject;
        Model = model;
        ReceivedValue = receivedValue;
        ValidationMessage = validationMessage;
    }

    /// <summary>
    /// Gets the associated symbol.
    /// </summary>
    public IValidatable Subject { get; }
    
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
        var identifier = GetSymbolIdentifier(helpProvider, eventInfo.Subject);
        var baseMessage = $"{identifier}: {validationMessage}";
        
        return new SymbolValidationError(
            eventInfo.Subject,
            eventInfo.Model,
            eventInfo.Value,
            validationMessage, 
            baseMessage);
    }
}