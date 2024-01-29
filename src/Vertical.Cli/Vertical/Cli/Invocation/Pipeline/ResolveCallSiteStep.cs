using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class ResolveCallSiteStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        PerformCore(state);

        next(state);
    }

    private void PerformCore(RuntimeState<TResult> state)
    {
        var subject = (ICommandDefinition<TResult>)state.BindingContext.Subject;
        var callSite = subject.CreateCallSite();

        state.CallSiteResult = new CallSiteContext<TResult>(
            callSite,
            state.Options,
            state.BindingContext);
    }
}