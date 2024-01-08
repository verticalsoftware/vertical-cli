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
        IEnumerable<string> args)
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

        var bindingPath = new BindingCommandPath<TResult>(commands, args, queue);
        var argumentBindings = new List<ArgumentBinding>(32);
        var thrown = default(Exception);

        try
        {
            argumentBindings.AddRange(bindingPath.SwitchSymbols.Select(symbol => symbol
                .BinderFactory().CreateBinding(bindingPath, symbol)));

            argumentBindings.AddRange(bindingPath.OptionSymbols.Select(symbol => symbol
                .BinderFactory().CreateBinding(bindingPath, symbol)));

            argumentBindings.AddRange(bindingPath.ArgumentSymbols.Select(symbol => symbol
                .BinderFactory().CreateBinding(bindingPath, symbol)));

            if (bindingPath.SemanticArguments.Any())
            {
                throw InvocationExceptions.InvalidArguments(bindingPath.SemanticArguments);
            }
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        return new BindingContext<TResult>(bindingPath, argumentBindings, thrown);
    }
}