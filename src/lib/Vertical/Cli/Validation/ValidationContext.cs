using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Validation;

/// <summary>
/// Represents the context of a validation event.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>
public sealed class ValidationContext<TModel, TValue> where TModel : class
{
    internal ValidationContext(
        ISymbolBinding symbolBinding,
        TModel model,
        TValue value,
        List<UsageError> errors)
    {
        _errors = errors;
        _symbolBinding = symbolBinding;
        
        Model = model;
        Value = value;
    }

    private readonly List<UsageError> _errors;
    private readonly ISymbolBinding _symbolBinding;

    /// <summary>
    /// Gets the symbol being validated.
    /// </summary>
    public ISymbol Symbol => _symbolBinding;

    /// <summary>
    /// Gets the composed model instance that will be passed to the command handler.
    /// </summary>
    public TModel Model { get; }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Adds a conversion error.
    /// </summary>
    /// <param name="exception">The exception thrown when the conversion failed.</param>
    public void AddConversionError(Exception? exception = null)
    {
        _errors.Add(new ConversionError(_symbolBinding, Value?.ToString() ?? string.Empty, exception));
    }

    /// <summary>
    /// Adds a custom validation error message.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void AddValidationError(string? message = null)
    {
        _errors.Add(new ValidationError(_symbolBinding, Value, message ?? "value is invalid"));
    }
}