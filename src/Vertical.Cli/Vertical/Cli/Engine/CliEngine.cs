using System.Diagnostics.CodeAnalysis;
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
    /// <returns></returns>
    public static BindingContext GetContext<TModel>(
        RootCommand<TModel> command,
        string[] arguments)
        where TModel : class
    {
        Guard.IsNotNull(command, nameof(command));
        Guard.IsNotNull(arguments, nameof(arguments));

        var preprocessedArguments = ArgumentPreProcessorPipeline.Invoke(
            arguments,
            command.Options.ArgumentPreProcessors);
        var argumentSyntaxList = BuildArgumentSyntaxList(command, preprocessedArguments);
        var queue = new Queue<ArgumentSyntax>(argumentSyntaxList);
        var path = new List<string>(6);
        
        // Resolve which command to invoke - peek/pick from leading arguments.
        CliCommand target = command;
        for (;;)
        {
            path.Add(target.PrimaryIdentifier);
            
            if (!queue.TryPeek(out var argument))
                break;

            if (!TryMatchSubCommand(target, argument.Text, out var child))
                break;

            target = child;
            queue.Dequeue();
        }

        var pathString = string.Join(' ', path);

        if (TryMatchModelessTaskConfiguration(target, queue, out var shortTask))
        {
            return new BindingContext(
                target,
                pathString,
                [],
                Enumerable.Empty<string>().ToLookup(_ => string.Empty),
                command.Options,
                shortTask.InvokeAsync(target, command.Options));
        }

        var symbols = target
            .AggregateSymbols()
            .ToArray();

        var valueLookup = ArgumentValueLookup.Create(queue, symbols);
        var options = command.Options;

        return new BindingContext(
            target, 
            pathString, 
            symbols, 
            valueLookup, 
            options);
    }

    private static List<ArgumentSyntax> BuildArgumentSyntaxList<TModel>(
        RootCommand<TModel> command,
        IReadOnlyCollection<string> arguments) where TModel : class
    {
        var syntaxList = ArgumentParser.Parse(arguments);
        
        command.Options.ArgumentTransform?.Invoke(syntaxList);
        
        return syntaxList;
    }

    private static bool TryMatchModelessTaskConfiguration(
        CliCommand command,
        Queue<ArgumentSyntax> queue,
        [NotNullWhen(true)] out ModelessTaskConfiguration? task)
    {
        task = null;
        
        if (!queue.TryPeek(out var argument))
            return false;

        var tasks = command.AggregateModelessTasks();

        task = tasks.FirstOrDefault(t => t.Names.Any(name => name == argument.Text));

        return task != null;
    }

    private static bool TryMatchSubCommand(
        CliCommand command,
        string argument,
        [NotNullWhen(true)] out CliCommand? match)
    {
        match = command
            .SubCommands
            .FirstOrDefault(cmd => cmd.Names.Any(name => name == argument));
        
        return match is not null;
    }
}