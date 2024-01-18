using Vertical.Cli.Binding;

namespace Vertical.Cli.Invocation;

internal sealed class CallSiteContext<TResult> : ICallSiteContext<TResult>
{
    internal CallSiteContext(
        ICallSite<TResult> callSite,
        CliOptions options,
        IBindingContext bindingContext,
        Exception? bindingException = null)
    {
        CallSite = callSite;
        BindingContext = bindingContext;
        BindingException = bindingException;
        Options = options;
    }

    /// <inheritdoc />
    public ICallSite<TResult> CallSite { get; }

    /// <inheritdoc />
    public IBindingContext BindingContext { get; }

    /// <inheritdoc />
    public Exception? BindingException { get; }

    /// <inheritdoc />
    public CliOptions Options { get; }

    /// <inheritdoc />
    public Func<CancellationToken, TResult> BindModelToCallSite<TModel>(
        Func<IBindingContext, TModel>? bindingFunction = null)
        where TModel : class
    {
        if (typeof(TModel) == typeof(None))
        {
            return CallSite.WrapParameter(None.Default);
        }

        TModel model;

        if (Options.TryCreateBinder<TModel>(out var binder))
        {
            model = binder.BindInstance(BindingContext);
            return CallSite.WrapParameter(model);
        }
        
        if (bindingFunction != null)
        {
            model = bindingFunction(BindingContext);
            return CallSite.WrapParameter(model);
        }

        throw new InvalidOperationException();
    }
}