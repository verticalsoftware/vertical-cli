using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation;

internal static class CommandSelector
{
    internal static (Queue<string> Arguments, ICommandDefinition<TResult> Subject) GetPath<TResult>(
        ICommandDefinition<TResult> rootCommand,
        IEnumerable<string> args)
    {
        ICommandDefinition<TResult> command = rootCommand;
        var queue = new Queue<string>(args);
        
        // Select command
        while (queue.TryPeek(out var arg))
        {
            if (!command.TryCreateChild(arg, out var child))
                break;

            queue.Dequeue();
            command = child;
        }

        return (queue, command);
    }
}