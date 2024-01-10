namespace Vertical.Cli.Invocation;

internal class CallSite<TResult> : ICallSite<TResult>
{
    private readonly object _callSiteDelegate;

    internal CallSite(
        object callSiteDelegate,
        bool isHelpSite,
        Type modelType)
    {
        _callSiteDelegate = callSiteDelegate;
        IsHelpSite = isHelpSite;
        ModelType = modelType;
    }

    public bool IsHelpSite { get; }

    public Type ModelType { get; }

    /// <inheritdoc />
    public Func<CancellationToken, TResult> WrapParameter<TModel>(TModel model)
    {
        var modelCallSite = (Func<TModel, CancellationToken, TResult>)_callSiteDelegate;

        return cancellationToken => modelCallSite(model, cancellationToken);
    }
}