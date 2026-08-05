namespace Vertical.Cli.Binding;

internal class PrivateBindingSource<TModel, TValue> : IBindingSource where TModel : class
{
    private readonly Func<PropertyBindingInfo, TValue> _valueProvider;

    private sealed class Binder(PrivateBindingSource<TModel, TValue> source) 
        : PropertyBinder
    {
        /// <inheritdoc />
        public override IBindingResult CreateBindingResult(PropertyBindingInfo bindingInfo)
        {
            return new BindingResult<TValue>(source.BindingName, source._valueProvider(bindingInfo));
        }
    }

    public PrivateBindingSource(string bindingName, 
        Func<PropertyBindingInfo, TValue> valueProvider,
        string description)
    {
        BindingName = bindingName;
        Description = description;

        _valueProvider = valueProvider;
    }

    /// <inheritdoc />
    public Type ModelType => typeof(TModel);

    /// <inheritdoc />
    public Type ValueType => typeof(TValue);

    /// <inheritdoc />
    public string BindingName { get; }

    /// <summary>
    /// Gets a binding description.
    /// </summary>
    public string Description { get; }

    /// <inheritdoc />
    public PropertyBinder CreatePropertyBinder() => new Binder(this);

    /// <inheritdoc />
    public override string ToString() => Description;
}