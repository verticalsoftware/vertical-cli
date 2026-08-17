using System.Linq.Expressions;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a context that can be used to retrieve property values for models.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class BindingContext<TModel> where TModel : class
{
    private readonly Dictionary<string, IBindingResult> _bindingResults;

    internal BindingContext(
        PropertyBindingInfo bindingInfo,
        Dictionary<string, IBindingResult> bindingResults)
    {
        BindingInfo = bindingInfo;
        _bindingResults = bindingResults;
    }

    /// <summary>
    /// Gets additional binding info.
    /// </summary>
    public PropertyBindingInfo BindingInfo { get; }

    /// <summary>
    /// Gets the value to bind to a model for the given property expression.
    /// </summary>
    /// <param name="expression">Expression that identifies the property.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Binding is not available in the context.</exception>
    /// <exception cref="InvalidOperationException">Binding is not the expected type..</exception>
    public TValue GetValue<TValue>(Expression<Func<TModel, TValue>> expression)
    {
        return GetBindingResult(expression).Value;
    }

    /// <summary>
    /// Gets the result that contains the value for the given property expression.
    /// </summary>
    /// <param name="expression">Expression that identifies the property.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Binding is not available in the context.</exception>
    /// <exception cref="InvalidOperationException">Binding is not the expected type..</exception>
    public BindingResult<TValue> GetBindingResult<TValue>(Expression<Func<TModel, TValue>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (!_bindingResults.TryGetValue(expression.BindingName, out var obj))
        {
            throw new InvalidOperationException($"Binding '{expression.BindingName}' is not available in the context.");
        }

        if (obj is not BindingResult<TValue> bindingResult)
        {
            throw new InvalidOperationException(
                $"Expected BindingResult<{typeof(TValue)}, but actual result was {obj.GetType()}");
        }

        return bindingResult;
    }
}