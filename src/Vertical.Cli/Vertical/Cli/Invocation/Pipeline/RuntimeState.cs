using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;

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

    internal RuntimeBindingContext BindingContext { get; private set; }

    internal CliOptions Options { get; }

    internal TResult DefaultResult { get; }
    
    internal ICallSiteContext<TResult>? CallSiteResult { get; set; }

    internal void MergeArguments(IEnumerable<string> arguments)
    {
        var mergedArguments = BindingContext
            .RawArguments
            .Concat(arguments)
            .ToArray();

        var selectedPath = CommandSelector.GetPath(
            (ICommandDefinition<TResult>)BindingContext.Subject.GetRootCommand(),
            mergedArguments);

        BindingContext = new RuntimeBindingContext(
            Options,
            selectedPath.Subject,
            mergedArguments,
            selectedPath.Arguments);
    }
}