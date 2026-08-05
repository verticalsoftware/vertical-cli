namespace Vertical.Cli.Validation;

public interface IValidationEventInfo<out TModel, out TValue> where TModel : class
{
    /// <summary>
    /// Gets a reference to the symbol being validated.
    /// </summary>
    IValidatable Subject { get; }

    /// <summary>
    /// Gets the constructed model reference.
    /// </summary>
    TModel Model { get; }

    /// <summary>
    /// Gets the subject value.
    /// </summary>
    TValue Value { get; }

    /// <summary>
    /// Returns a reference to this instance.
    /// </summary>
    IValidationEventInfo<TModel, TValue> OK { get; }

    /// <summary>
    /// Reports a validation error.
    /// </summary>
    /// <param name="message">The message to report.</param>
    /// <returns>A reference to this instance.</returns>
    IValidationEventInfo<TModel, TValue> Error(string message);
}

public interface IValidationEventInfo<out TModel, out TElement, out TCollection> : IValidationEventInfo<TModel, TCollection>
    where TModel : class
    where TCollection : IEnumerable<TElement>
{
    /// <summary>
    /// Invokes the given value check action with each value in the collection.
    /// </summary>
    /// <param name="validate">
    /// An action that evaluates the model and/or subject value.
    /// </param>
    /// <returns>The provided context object.</returns>
    public IValidationEventInfo<TModel, TElement, TCollection> EachValue(
        Action<IValidationEventInfo<TModel, TElement>> validate);
}