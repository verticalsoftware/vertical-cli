﻿using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation.Pipeline;

internal sealed class PostCheckArgumentsStep<TResult> : CallSiteBuilderStep<TResult>
{
    /// <inheritdoc />
    internal override void Perform(RuntimeState<TResult> state, Action<RuntimeState<TResult>> next)
    {
        var unacceptedArguments = state
            .BindingContext
            .SemanticArguments
            .Unaccepted
            .ToArray();

        if (unacceptedArguments.Any())
        {
            throw InvocationExceptions.InvalidArguments(unacceptedArguments);
        }
    }
}