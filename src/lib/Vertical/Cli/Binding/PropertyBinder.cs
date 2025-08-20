using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents data used to realize a model's property value.
/// </summary>
/// <typeparam name="TValue">Value type</typeparam>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class PropertyBinder<TModel, TValue> where TModel : class
{
    internal PropertyBinder(
        IPropertyBinding<TModel, TValue> propertyBinding,
        BindingContext<TModel> bindingContext,
        ValueConverter<TValue>? valueConverter)
    {
        PropertyBinding = propertyBinding;
        BindingContext = bindingContext;
        ValueConverter = valueConverter;
    }

    private enum ValueSource
    {
        Default,
        Explicit
    };

    private (ValueSource Source, TValue Value)? _result;

    /// <summary>
    /// Gets the property binding.
    /// </summary>
    public IPropertyBinding<TModel, TValue> PropertyBinding { get; }

    /// <summary>
    /// Gets the binding context.
    /// </summary>
    public BindingContext<TModel> BindingContext { get; }

    /// <summary>
    /// Gets whether a result has been set.
    /// </summary>
    public bool IsValueSet => _result.HasValue;

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    public ParseResult ParseResult => BindingContext.ParseResult;

    /// <summary>
    /// Gets the values for the property binding.
    /// </summary>
    /// <returns><c>string[]</c></returns>
    public string[] GetValues() => BindingContext
        .ParseResult
        .GetValues(PropertyBinding.BindingName);
    
    /// <summary>
    /// Gets the default value converter or <c>null</c>.
    /// </summary>
    public ValueConverter<TValue>? ValueConverter { get; }

    /// <summary>
    /// Configures the binding with a value to use if no other value can be determined.
    /// </summary>
    /// <param name="defaultValue">The default value to use.</param>
    public void SetDefaultValue(TValue defaultValue)
    {
        _result = (ValueSource.Default, defaultValue);
    }

    /// <summary>
    /// Configures the binding to explicitly use the provided value.
    /// </summary>
    /// <param name="value">The value to use.</param>
    public void SetValue(TValue value)
    {
        _result = (ValueSource.Explicit, value);
    }

    internal bool TryGetDefaultValue(out TValue value) => TryGetValue(ValueSource.Default, out value);

    internal bool TryGetExplicitValue(out TValue value) => TryGetValue(ValueSource.Explicit, out value);

    private bool TryGetValue(ValueSource source, out TValue value)
    {
        value = _result.HasValue ? _result.Value.Value : default!;
        return _result?.Source == source;
    }
}