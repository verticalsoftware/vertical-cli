using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

internal static class HelpCallSite
{
    internal static bool TryCreate<TResult>(
        BindingCreateContext<TResult> createContext,
        ICollection<ArgumentBinding> argumentBindings,
        TResult defaultValue,
        [NotNullWhen(true)] out ICallSite<TResult>? callSite)
    {
        callSite = null;

        if (createContext.HelpOptionSymbol == null)
            return false;

        var helpSymbol = (HelpSymbolDefinition<TResult>)createContext.HelpOptionSymbol;
        var binding = (ArgumentBinding<bool>)helpSymbol.CreateBinding(createContext);

        if (!binding.Values.Any(set => set))
            return false;

        argumentBindings.Add(binding);

        var callSiteDelegate = (None _, CancellationToken _) =>
        {
            var options = createContext.Subject.Options;
            var renderer = options.CreateHelpEngine();
            renderer.WriteContent(createContext.Subject);

            return helpSymbol.ReturnValue ?? defaultValue;
        };

        callSite = CallSite.Create(callSiteDelegate, isHelpSite: true);

        return true;
    }
}