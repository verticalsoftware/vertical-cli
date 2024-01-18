using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Binding;

/// <summary>
/// Implements a binder that uses a delegate implementation.
/// </summary>
/// <typeparam name="T">Model type.</typeparam>
public sealed class DelegatedBinder<T> : ModelBinder<T>
{
    private readonly Func<IBindingContext, T> _binder;

    public DelegatedBinder(Func<IBindingContext, T> binder)
    {
        Guard.IsNotNull(binder);
        _binder = binder;
    }

    /// <inheritdoc />
    public override T BindInstance(IBindingContext bindingContext) => _binder(bindingContext);
}