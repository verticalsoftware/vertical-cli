using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Creates a <see cref="ICallSiteContext{TResult}"/> from a root command definition and program arguments.
/// </summary>
public static class CallSiteContext
{
    /// <summary>
    /// Creates a call site.
    /// </summary>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="args">Application arguments.</param>
    /// <param name="defaultValue">The default value to return for help or error sites.</param>
    /// <typeparam name="TModel">Root command model type.</typeparam>
    /// <typeparam name="TResult">Application result type.</typeparam>
    /// <returns><see cref="ICallSiteContext{TResult}"/></returns>
    public static ICallSiteContext<TResult> Create<TModel, TResult>(
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

        var subject = commands.Last();
        var bindingContext = new RuntimeBindingContext(rootCommand.Options, subject, args, queue);
        var options = rootCommand.Options;

        try
        {
            if (HelpCallSite.TryCreate(bindingContext, options, defaultValue, out var helpSite))
            {
                return new CallSiteContext<TResult>(helpSite, options, bindingContext);
            }

            var commandSite = CreateCommandCallSite<TResult>(bindingContext);

            if (bindingContext.SemanticArguments.Unaccepted.Any())
            {
                // Any arguments left not mapped to symbols are invalid
                throw InvocationExceptions.InvalidArguments(bindingContext.SemanticArguments.Unaccepted);
            }

            return new CallSiteContext<TResult>(commandSite, options, bindingContext);
        }
        catch (Exception exception)
        {
            return new CallSiteContext<TResult>(
                ExceptionStateCallSite.Create(exception, rootCommand.Options, defaultValue),
                options,
                bindingContext,
                exception);
        }
    }

    private static ICallSite<TResult> CreateCommandCallSite<TResult>(RuntimeBindingContext bindingContext)
    {
        // These have to be done in a specific order so the arguments
        // are mapped and consumed properly
        bindingContext.AddBindings(CreateBindings(bindingContext, bindingContext.SwitchSymbols));
        bindingContext.AddBindings(CreateBindings(bindingContext, bindingContext.OptionSymbols));
        bindingContext.AddBindings(CreateBindings(bindingContext, bindingContext.ArgumentSymbols));

        var subject = (ICommandDefinition<TResult>)bindingContext.Subject;

        return subject.CreateCallSite();
    }

    private static IEnumerable<ArgumentBinding> CreateBindings(
        IBindingContext bindingContext,
        IEnumerable<SymbolDefinition> symbols)
    {
        return symbols.Select(symbol => symbol.CreateBinding(bindingContext));
    }
}