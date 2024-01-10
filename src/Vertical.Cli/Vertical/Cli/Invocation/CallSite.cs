using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Creates a call site.
/// </summary>
internal static class CallSite
{
    internal static CallSite<TResult> Create<TModel, TResult>(
        Func<TModel, CancellationToken, TResult> handler, 
        bool isHelpSite)
    {
        return new CallSite<TResult>(handler, isHelpSite, typeof(TModel));
    }
    
    private static ICallSite<TResult> CreateCommandCallSite<TResult>(
        BindingCreateContext<TResult> createContext,
        List<ArgumentBinding> argumentBindings)
    {
        // These have to be done in a specific order so the arguments
        // are mapped and consumed properly
        argumentBindings.AddRange(CreateBindings(createContext, createContext.SwitchSymbols));
        argumentBindings.AddRange(CreateBindings(createContext, createContext.OptionSymbols));
        argumentBindings.AddRange(CreateBindings(createContext, createContext.ArgumentSymbols));

        var subject = createContext.Subject;

        return subject.CreateCallSite();
    }
    
    private static IEnumerable<ArgumentBinding> CreateBindings(
        IBindingCreateContext bindingCreateContext,
        IEnumerable<SymbolDefinition> symbols)
    {
        return symbols.Select(symbol => symbol.CreateBinding(bindingCreateContext));
    }
}