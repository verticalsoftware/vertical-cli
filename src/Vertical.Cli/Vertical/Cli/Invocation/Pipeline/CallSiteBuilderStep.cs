namespace Vertical.Cli.Invocation.Pipeline;

internal abstract class CallSiteBuilderStep<TResult>
{
    internal abstract void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next);
}