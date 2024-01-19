using Vertical.Cli.Binding;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class RuntimeState<TResult>
{
    internal RuntimeState(
        RuntimeBindingContext bindingContext,
        CliOptions options,
        TResult defaultResult)
    {
        BindingContext = bindingContext;
        Options = options;
        DefaultResult = defaultResult;
    }

    internal RuntimeBindingContext BindingContext { get; set; }

    internal CliOptions Options { get; }

    internal TResult DefaultResult { get; }
    
    internal ICallSiteContext<TResult>? CallSiteResult { get; set; }
}