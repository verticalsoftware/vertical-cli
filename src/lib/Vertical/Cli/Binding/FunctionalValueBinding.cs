namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a binding that uses a static value.
/// </summary>
/// <typeparam name="TValue">Value type</typeparam>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class FunctionalValueBinding<TModel, TValue> : IPropertyBinding<TModel, TValue> 
    where TModel : class
{
    internal FunctionalValueBinding(Type modelType, 
        string bindingName, 
        Action<PropertyBinder<TModel, TValue>> configureBinding)
    {
        _configureBinding = configureBinding;
        
        ModelType = modelType;
        BindingName = bindingName;
    }

    private readonly Action<PropertyBinder<TModel, TValue>> _configureBinding;

    /// <inheritdoc />
    public bool TryBindValue(PropertyBinder<TModel, TValue> binder)
    {
        _configureBinding(binder);
        return binder.IsValueSet;
    }

    /// <inheritdoc />
    public string BindingName { get; }

    /// <inheritdoc />
    public Type ModelType { get; }

    /// <inheritdoc />
    public Type ValueType => typeof(TValue);

    /// <inheritdoc />
    public bool HasBindingOptions => true;
}