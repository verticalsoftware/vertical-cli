using System.Linq.Expressions;
using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an object used to configure model binding.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IModelBuilder<TModel> where TModel : class
{
    /// <summary>
    /// Sets a preconfigured private value for a model's property.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="value">The static value to map into new model instances.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> MapStaticValue<TValue>(Expression<Func<TModel, TValue>> expression, TValue value);

    /// <summary>
    /// Sets a preconfigured private binding value for a model's property.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="valueProvider">A delegate that provides the value to bind.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> MapBindingInfoValue<TValue>(
        Expression<Func<TModel, TValue>> expression,
        Func<PropertyBindingInfo, TValue> valueProvider);

    /// <summary>
    /// Sets a <see cref="TextReader"/> property of model to the console abstraction's
    /// input text reader.
    /// </summary>
    /// <param name="expression">Expression that identifies a model's <see cref="TextReader"/> property.</param>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> MapTextReader(Expression<Func<TModel, TextReader>> expression);

    /// <summary>
    /// Establishes the action that creates instances of the model type.
    /// </summary>
    /// <param name="binder">
    /// An action that uses the parse result to build new instances of the model type.
    /// </param>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> SetBinder(ModelBinder<TModel> binder);

    /// <summary>
    /// Sets a <see cref="TextWriter"/> property of a model to the console abstraction's
    /// input text reader.
    /// </summary>
    /// <param name="expression">Expression that identifies a model's <see cref="TextReader"/> property.</param>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> MapTextWriter(Expression<Func<TModel, TextWriter>> expression);
}