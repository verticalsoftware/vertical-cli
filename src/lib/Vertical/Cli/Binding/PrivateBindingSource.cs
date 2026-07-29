namespace Vertical.Cli.Binding;

internal class PrivateBindingSource<TValue> : IBindingSource
{
    private readonly Func<PropertyBindingInfo, TValue> _valueProvider;

    private sealed class Binder(PrivateBindingSource<TValue> source) : PropertyBinder
    {
        /// <inheritdoc />
        public override IBindingResult CreateBindingResult(PropertyBindingInfo bindingInfo)
        {
            return new BindingResult<TValue>(source.BindingName, source._valueProvider(bindingInfo));
        }
    }

    public PrivateBindingSource(string bindingName, Func<PropertyBindingInfo, TValue> valueProvider)
    {
        BindingName = bindingName;
        _valueProvider = valueProvider;
    }
    
    /// <inheritdoc />
    public Type ValueType => typeof(TValue);

    /// <inheritdoc />
    public string BindingName { get; }

    /// <inheritdoc />
    public PropertyBinder CreatePropertyBinder() => new Binder(this);
}