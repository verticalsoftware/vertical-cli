using Vertical.Cli.Binding;

namespace Vertical.Cli.Invocation.Pipeline;

internal static class CallSiteBuilder
{
    internal static ICallSiteContext<TResult> Build<TResult>(
        RuntimeBindingContext bindingContext,
        CliOptions options,
        TResult defaultResult)
    {
        var buildContext = new RuntimeState<TResult>(
            bindingContext,
            options,
            defaultResult);

        var pipeline = CreatePipeline<TResult>();

        pipeline(buildContext);

        return buildContext.CallSiteResult ?? throw new InvalidOperationException();
    }

    private static Action<RuntimeState<TResult>> CreatePipeline<TResult>()
    {
        var call = new Action<RuntimeState<TResult>>(_ => { });

        return CreatePipelineSteps<TResult>()
            .Reverse()
            .Aggregate(call, (current, next) => context => next.Perform(context, current));
    }

    private static IEnumerable<CallSiteBuilderStep<TResult>> CreatePipelineSteps<TResult>()
    {
        return new CallSiteBuilderStep<TResult>[]
        {
            new ReadResponseFileStep<TResult>(),
            new EvaluateHelpOptionStep<TResult>(),
            new CatchErrorSiteStep<TResult>(),
            new AddBindingsStep<TResult>(),
            new PostCheckArgumentsStep<TResult>(),
            new ResolveCallSiteStep<TResult>()
        };
    }
}