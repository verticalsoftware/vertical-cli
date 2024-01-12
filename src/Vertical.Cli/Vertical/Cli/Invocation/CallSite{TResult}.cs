using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation;

internal class CallSite<TResult> : ICallSite<TResult>
{
    private readonly object _callSiteDelegate;

    internal static ICallSite<TResult> Create<TModel>(
        ICommandDefinition<TModel, TResult> subject,
        Func<TModel, CancellationToken, TResult> callSiteDelegate,
        CallState state) 
        where TModel : class
    {
        return new CallSite<TResult>(subject, callSiteDelegate, state);
    }

    private CallSite(
        ICommandDefinition<TResult> subject,
        object callSiteDelegate,
        CallState state)
    {
        _callSiteDelegate = callSiteDelegate;
        State = state;
        Subject = subject;
    }

    /// <inheritdoc />
    public CallState State { get; }

    /// <inheritdoc />
    public Type ModelType => Subject.ModelType;

    /// <inheritdoc />
    public ICommandDefinition<TResult> Subject { get; }

    /// <inheritdoc />
    public Func<CancellationToken, TResult> WrapParameter<TModel>(TModel model)
    {
        var modelCallSite = (Func<TModel, CancellationToken, TResult>)_callSiteDelegate;
        return cancellationToken => modelCallSite(model, cancellationToken);
    }
}