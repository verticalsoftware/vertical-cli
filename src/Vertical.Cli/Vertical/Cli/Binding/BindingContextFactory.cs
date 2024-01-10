using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
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
        var throwException = default(Exception);
        ICallSite<TResult>? callSite;

        try
        {
            if (!HelpCallSite.TryCreate(createContext, argumentBindings, defaultValue, out callSite))
            {
                callSite = CreateCommandCallSite(createContext, argumentBindings);
            }

            if (createContext.SemanticArguments.Any())
            {
                // Any arguments left not mapped to symbols are invalid
                throw InvocationExceptions.InvalidArguments(createContext.SemanticArguments);
            }
        }
        catch (Exception exception)
        {
            throwException = exception;
            callSite = ExceptionStateCallSite.Create(exception, command.Options, defaultValue);
        }

        return new BindingContext<TResult>(createContext, argumentBindings, callSite, throwException);
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