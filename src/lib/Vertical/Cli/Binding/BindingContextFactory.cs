using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Creates a <see cref="IBindingContext{TResult}"/> from a root command definition and program arguments.
/// </summary>
internal static class BindingContextFactory
{
    internal static IBindingContext<TResult> Create<TModel, TResult>(
        IRootCommand<TModel, TResult> rootCommand,
        IEnumerable<string> args,
        TResult defaultValue)
        where TModel : class
    {
        Guard.IsNotNull(rootCommand);
        Guard.IsNotNull(args);

        ICommandDefinition<TResult> command = rootCommand;
        var queue = new Queue<string>(args);
        var commands = new List<ICommandDefinition<TResult>>(5) { rootCommand };

        // Select command
        while (queue.TryPeek(out var arg))
        {
            if (!command.TryCreateChild(arg, out var child))
                break;

            queue.Dequeue();
            commands.Add(command = child);
        }

        var createContext = new BindingCreateContext<TResult>(commands, args, queue);
        var argumentBindings = new List<ArgumentBinding>(32);
        var thrown = default(Exception);
        Func<TResult>? helpCallSite = null;

        try
        {
            if (!TryCreateHelpSite(createContext, argumentBindings, defaultValue, out helpCallSite))
            {
                // These have to be done in a specific order so the arguments
                // are mapped and consumed properly
                argumentBindings.AddRange(CreateBindings(createContext, createContext.SwitchSymbols));
                argumentBindings.AddRange(CreateBindings(createContext, createContext.OptionSymbols));
                argumentBindings.AddRange(CreateBindings(createContext, createContext.ArgumentSymbols));
            }

            if (createContext.SemanticArguments.Any())
            {
                // Any arguments left not mapped to symbols are invalid
                throw InvocationExceptions.InvalidArguments(createContext.SemanticArguments);
            }
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        return new BindingContext<TResult>(createContext, argumentBindings, helpCallSite, thrown);
    }

    private static bool TryCreateHelpSite<TResult>(
        BindingCreateContext<TResult> createContext,
        ICollection<ArgumentBinding> argumentBindings,
        TResult defaultValue,
        out Func<TResult>? helpCallSite)
    {
        helpCallSite = null;

        if (createContext.HelpOptionSymbol == null)
            return false;

        var helpSymbol = (HelpSymbolDefinition<TResult>)createContext.HelpOptionSymbol;
        var binder = helpSymbol.BindingProvider();
        var binding = (ArgumentBinding<bool>)binder.CreateBinding(createContext, helpSymbol);

        if (!binding.Values.Any(set => set))
            return false;
        
        argumentBindings.Add(binding);

        helpCallSite = () =>
        {
            helpSymbol.HelpRenderer.WriteContent(createContext.Subject);
            return helpSymbol.ReturnValue ?? defaultValue;
        };

        return true;
    }

    private static IEnumerable<ArgumentBinding> CreateBindings(
        IBindingCreateContext bindingCreateContext,
        IEnumerable<SymbolDefinition> symbols)
    {
        return symbols.Select(symbol => symbol.BindingProvider().CreateBinding(bindingCreateContext, symbol));
    }
}