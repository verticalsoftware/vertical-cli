using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Engine;

/// <summary>
/// Main entry point for generated code.
/// </summary>
public static class CliEngine
{
    /// <summary>
    /// Parses the input arguments and returns a context that be used for model binding.
    /// </summary>
    /// <param name="command">The command definition.</param>
    /// <param name="arguments">Arguments of the application.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <returns></returns>
    public static BindingContext<TResult> GetContext<TModel, TResult>(
        RootCommand<TModel, TResult> command,
        string[] arguments)
        where TModel : class
    {
        Guard.IsNotNull(command, nameof(command));
        Guard.IsNotNull(arguments, nameof(arguments));
        
        var argumentSyntaxes = ArgumentParser.Parse(arguments);
        var queue = new Queue<ArgumentSyntax>(argumentSyntaxes);
        var path = new List<string>(6);

        // Resolve which command to invoke - peek/pick from leading arguments.
        CliCommand<TResult> target = command;
        for (;;)
        {
            path.Add(target.PrimaryIdentifier);
            
            if (!queue.TryPeek(out var argument))
                break;

            // if (argument.PrefixType != OptionPrefixType.None)
            //     break;

            var child = target
                .Commands
                .FirstOrDefault(cmd => cmd.Names.Any(name => name == argument.Text));
            
            if (child == null)
                break;

            target = (CliCommand<TResult>)child;
            queue.Dequeue();
        }

        var symbols = target
            .AggregateSymbols()
            .ToArray();

        var valueLookup = ArgumentValueLookup.Create(queue, symbols);

        return new BindingContext<TResult>(
            target, 
            string.Join(' ', path), 
            symbols, 
            valueLookup, 
            command.Options);
    }
}