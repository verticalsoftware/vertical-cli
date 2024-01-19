using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class BuildCommandSiteStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        var context = state.BindingContext;
        
        context.AddBindings(context.SwitchSymbols.Select(symbol => symbol.CreateBinding(context)));
        context.AddBindings(context.OptionSymbols.Select(symbol => symbol.CreateBinding(context)));
        context.AddBindings(context.ArgumentSymbols.Select(symbol => symbol.CreateBinding(context)));

        var subject = (ICommandDefinition<TResult>)context.Subject;
        var callSite = subject.CreateCallSite();

        state.CallSiteResult = new CallSiteContext<TResult>(
            callSite,
            state.Options,
            state.BindingContext);
    }
}