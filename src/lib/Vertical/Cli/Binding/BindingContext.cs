using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a context used to provide values for model activation.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class BindingContext<TModel> where TModel : class
{
    internal BindingContext(
        IRootConfiguration rootConfiguration,
        IEnumerable<IPropertyBinding> propertyBindings,
        ParseResult parseResult,
        List<UsageError> errors,
        Func<TModel>? activator,
        TextReader inputStream)
    {
        _rootConfiguration = rootConfiguration;
        _errors = errors;

        ParseResult = parseResult;
        Activator = activator;
        PropertyBindings = propertyBindings.ToDictionary(
            binding => binding.BindingName, 
            IPropertyBinding (binding) => binding);
        InputStream = inputStream;
    }

    private readonly IRootConfiguration _rootConfiguration;
    private readonly List<UsageError> _errors;

    /// <summary>
    /// Gets the input stream.
    /// </summary>
    public TextReader InputStream { get; }

    /// <summary>
    /// Gets a dictionary of <see cref="IPropertyBinding"/> objects indexed by binding name.
    /// </summary>
    public Dictionary<string, IPropertyBinding> PropertyBindings { get; set; }

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    public ParseResult ParseResult { get; }

    /// <summary>
    /// Gets a factory function that creates the initial object instance.
    /// </summary>
    public Func<TModel>? Activator { get; }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <param name="propertyExpression">The expression that identifies the model property being bound.</param>
    /// <param name="valueConverter">A function that converts string arguments to the target value type.</param>
    /// <typeparam name="TValue">Property type</typeparam>
    /// <returns><typeparamref name="TValue"/> or <c>default</c>.</returns>
    public TValue GetValue<TValue>(
        Expression<Func<TModel, TValue>> propertyExpression,
        ValueConverter<TValue>? valueConverter)
    {
        var binding = GetPropertyBinding(propertyExpression);
        var bindingArgs = new PropertyBinder<TModel, TValue>(binding, this, valueConverter);
        binding.TryBindValue(bindingArgs);

        if (bindingArgs.TryGetExplicitValue(out var value))
            return value;
        
        if (TryGetSingleParseResultValue(binding, valueConverter, out var parsedValue))
            return parsedValue;

        return bindingArgs.TryGetDefaultValue(out var defaultValue) ? defaultValue : default!;
    }

    /// <summary>
    /// Gets a collection property value.
    /// </summary>
    /// <param name="propertyExpression">The expression that identifies the model property being bound.</param>
    /// <param name="elementConverter">A function that converts string arguments to the target element type.</param>
    /// <param name="createCollection">A function that converts enumerable values to the strong collection type.</param>
    /// <typeparam name="TElement">The collection or array's element type.</typeparam>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <returns>The collection value</returns>
    public TCollection GetCollectionValue<TElement, TCollection>(
        Expression<Func<TModel, TCollection>> propertyExpression,
        ValueConverter<TElement>? elementConverter,
        Func<IEnumerable<TElement>, TCollection> createCollection)
        where TCollection : IEnumerable<TElement>
    {
        var binding = GetPropertyBinding(propertyExpression);
        var bindingArgs = new PropertyBinder<TModel, TCollection>(binding, this, null);

        binding.TryBindValue(bindingArgs);

        if (bindingArgs.TryGetExplicitValue(out var value))
            return value;

        if (TryGetCollectionParseResultValue(binding, elementConverter, createCollection, out var collection))
            return collection;

        return bindingArgs.TryGetDefaultValue(out collection) ? collection : default!;
    }

    /// <summary>
    /// Creates an instance of the model type.
    /// </summary>
    /// <returns>An instance of the model.</returns>
    /// <exception cref="InvalidOperationException">An activator function was not configured.</exception>
    public TModel ActivateInstance()
    {
        return Activator?.Invoke() ?? throw Exceptions.ModelActivatorNotDefined(typeof(TModel));
    }

    private bool TryGetSingleParseResultValue<TValue>(
        IPropertyBinding<TModel, TValue> binding,
        ValueConverter<TValue>? valueConverter,
        out TValue value)
    {
        value = default!;

        if (ParseResult.GetValues(binding.BindingName) is not { Length: 1 } parseValues)
            return false;

        var converter = ResolveConverter(binding, valueConverter);

        return TryConvertValue(binding, parseValues[0], converter, out value);
    }

    private bool TryGetCollectionParseResultValue<TCollection, TElement>(
        IPropertyBinding<TModel, TCollection> binding, 
        ValueConverter<TElement>? elementConverter, 
        Func<IEnumerable<TElement>, TCollection> createCollection, 
        [NotNullWhen(true)] out TCollection? collection)
        where TCollection : IEnumerable<TElement>
    {
        collection = default!;

        if (ParseResult.GetValues(binding.BindingName) is not { Length: > 0 } parseValues)
            return false;

        var converter = ResolveConverter(binding, elementConverter);
        var valueArray = new TElement[parseValues.Length];

        for (var c = 0; c < valueArray.Length; c++)
        {
            if (!TryConvertValue(binding, parseValues[c], converter, out var value))
                return false;

            valueArray[c] = value;
        }

        collection = valueArray.GetType().IsAssignableTo(typeof(TCollection))
            ? (TCollection)(object)valueArray
            : createCollection(valueArray);
        
        return true;
    }

    private bool TryConvertValue<TValue>(
        IPropertyBinding binding,
        string value, 
        ValueConverter<TValue> valueConverter, 
        out TValue convertedValue)
    {
        convertedValue = default!;

        try
        {
            convertedValue = valueConverter(value);
            return true;
        }
        catch (Exception exception)
        {
            _errors.Add(new ConversionError(binding, value, exception));
            return false;
        }
    }

    private IPropertyBinding<TModel, TValue> GetPropertyBinding<TValue>(Expression<Func<TModel, TValue>> propertyExpression)
    {
        var bindingName = propertyExpression.GetPropertyName();

        return PropertyBindings.GetValueOrDefault(bindingName) switch
        {
            IPropertyBinding<TModel, TValue> typedBinding => typedBinding,
            { } binding => throw Exceptions.InvalidBindingCast(typeof(TModel), typeof(TValue), binding),
            _ => throw Exceptions.InvalidBindingName(typeof(TModel), typeof(TValue), bindingName)
        };
    }

    private ValueConverter<TValue> ResolveConverter<TValue>(
        IPropertyBinding propertyBinding,
        ValueConverter<TValue>? providedConverter)
    {
        if (_rootConfiguration.TryGetValueConverter<TValue>(out var converter))
            return converter;

        return providedConverter ?? throw Exceptions.ValueConverterNotDefined(propertyBinding);
    }
}