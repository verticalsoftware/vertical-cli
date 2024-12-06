namespace Vertical.Cli.Binding;

internal sealed class WrappedBindingSource(Func<object> value) : IBindingSource
{
    /// <inheritdoc />
    public bool TryGetValue(out object? obj)
    {
        obj = value();
        return true;
    }
}