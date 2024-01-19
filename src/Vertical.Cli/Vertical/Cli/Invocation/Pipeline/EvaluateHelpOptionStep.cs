using Vertical.Cli.Help;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class EvaluateHelpOptionStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        if (!HelpCallSite.TryCreate(
                state.BindingContext,
                state.Options,
                state.DefaultResult,
                out var callSite))
        {
            next(state);
            return;
        }

        state.CallSiteResult = new CallSiteContext<TResult>(
            callSite,
            state.Options,
            state.BindingContext);
    }
}