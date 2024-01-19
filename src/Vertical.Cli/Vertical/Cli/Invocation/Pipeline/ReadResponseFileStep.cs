namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class ReadResponseFileStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        next(state);
    }
}