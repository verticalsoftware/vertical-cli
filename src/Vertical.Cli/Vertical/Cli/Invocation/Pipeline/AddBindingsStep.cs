using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class AddBindingsStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        PerformCore(state);

        next(state);
    }

    private static void PerformCore(RuntimeState<TResult> state)
    {
        var context = state.BindingContext;
        
        context.AddBindings(context.SwitchSymbols.Select(symbol => symbol.CreateBinding(context)));
        context.AddBindings(context.OptionSymbols.Select(symbol => symbol.CreateBinding(context)));
        context.AddBindings(context.ArgumentSymbols.Select(symbol => symbol.CreateBinding(context)));
    }
}