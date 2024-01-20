﻿using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Help;

internal static class HelpCallSite
{
    internal static bool TryCreate<TResult>(
        RuntimeBindingContext bindingContext,
        CliOptions options,
        TResult defaultValue,
        [NotNullWhen(true)] out ICallSite<TResult>? callSite)
    {
        callSite = null;

        if (bindingContext.HelpOptionSymbol == null)
            return false;

        var helpSymbol = (SymbolDefinition<bool>)bindingContext.HelpOptionSymbol;
        var binding = (ArgumentBinding<bool>)helpSymbol.CreateBinding(bindingContext);

        if (!binding.Values.Any(set => set))
            return false;

        bindingContext.AddBinding(binding);

        callSite = CallSite<TResult>.Create(
            new EmptyCommandDefinition<TResult>(options),
            (_, _) => InvokeHelp(options, bindingContext, defaultValue),
            CallState.Help);

        return true;
    }

    private static TResult InvokeHelp<TResult>(
        CliOptions options,
        IBindingContext bindingContext,
        TResult defaultValue)
    {
        var formatter = options.CreateHelpFormatter();
        formatter.WriteContent(bindingContext.Subject);

        return defaultValue;
    }
}