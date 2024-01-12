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
    public Func<CancellationToken, TResult> BindModelParameter<TModel>() where TModel : class
    {
        var binder = (ModelBinder<TModel>)Options
            .ModelBinders
            .First(item => item.ModelType == typeof(TModel));

        var model = binder.BindInstance(BindingContext);

        return CallSite.WrapParameter(model);
    }

    /// <inheritdoc />
    public Func<CancellationToken, TResult> BindModelParameter<TModel>(
        Func<IBindingContext, TModel> bindingFunction)
        where TModel : class
    {
        if (typeof(TModel) == typeof(None))
        {
            return CallSite.WrapParameter(None.Default);
        }
        
        var model = bindingFunction(BindingContext);            

        return CallSite.WrapParameter(model);
    }
}