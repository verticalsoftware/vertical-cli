namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class CatchErrorSiteStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        try
        {
            next(state);
        }
        catch (Exception exception)
        {
            state.CallSiteResult = new CallSiteContext<TResult>(
                ExceptionStateCallSite.Create(exception, state.Options, state.DefaultResult),
                state.Options,
                state.BindingContext,
                exception);
        }
    }
}