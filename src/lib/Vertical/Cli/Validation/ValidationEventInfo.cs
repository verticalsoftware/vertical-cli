using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Validation;

/// <summary>
/// Represents validation data for a property.
/// </summary>
/// <typeparam name="TModel">The model type the property is a member of.</typeparam>
/// <typeparam name="TValue">The property value type.</typeparam>
public class ValidationEventInfo<TModel, TValue> : IValidationEventInfo<TModel, TValue> where TModel : class
{
    /// <summary>
    /// Gets the validation context.
    /// </summary>
    protected ValidationContext Context { get; }

    internal ValidationEventInfo(
        ValidationContext context, 
        IValidatable subject,
        TModel model,
        TValue value)
    
    {
        Context = context;
        Subject = subject;
        Model = model;
        Value = value;
    }

    /// <summary>
    /// Gets a reference to the symbol being validated.
    /// </summary>
    public IValidatable Subject { get; }

    /// <summary>
    /// Gets the constructed model reference.
    /// </summary>
    public TModel Model { get; }

    /// <summary>
    /// Gets the subject value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Returns a reference to this instance.
    /// </summary>
    public IValidationEventInfo<TModel, TValue> OK => this;

    /// <summary>
    /// Reports a validation error.
    /// </summary>
    /// <param name="message">The message to report.</param>
    /// <returns>A reference to this instance.</returns>
    public IValidationEventInfo<TModel, TValue> Error(string message)
    {
        Context.AddError(SymbolValidationError.Create(this, message, Context.HelpProvider));
        return this;
    }
}

/// <summary>
/// Represents validation data for a property.
/// </summary>
/// <typeparam name="TModel">The model type the property is a member of.</typeparam>
/// <typeparam name="TElement">The collection's element type.</typeparam>
/// <typeparam name="TCollection">The property type.</typeparam>
public sealed class ValidationEventInfo<TModel, TElement, TCollection> : 
    ValidationEventInfo<TModel, TCollection>,
    IValidationEventInfo<TModel, TElement, TCollection> where TModel : class
    where TCollection : IEnumerable<TElement>
{
    /// <inheritdoc />
    internal ValidationEventInfo(
        ValidationContext context,
        CliSymbol<TModel, TCollection> symbol,
        TModel model,
        TCollection value)
        : base(context, symbol, model, value)
    {
    }

    /// <summary>
    /// Invokes the given value check action with each value in the collection.
    /// </summary>
    /// <param name="validate">
    /// An action that evaluates the model and/or subject value.
    /// </param>
    /// <returns>The provided context object.</returns>
    public IValidationEventInfo<TModel, TElement, TCollection> EachValue(
        Action<IValidationEventInfo<TModel, TElement>> validate)
    {
        foreach (var value in Value)
        {
            var elementInfo = new ValidationEventInfo<TModel, TElement>(Context, Subject, Model, value);
            validate(elementInfo);
        }

        return this;
    }
}