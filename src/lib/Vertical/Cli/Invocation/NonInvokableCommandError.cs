using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents the error that occurs when the client's argument target a command that itself
/// is not invokable.
/// </summary>
public sealed class NonInvokableCommandError : UsageError
{
    internal NonInvokableCommandError(ICommand command)
    {
        Command = command;
    }

    /// <summary>
    /// Gets the non-invokable command.
    /// </summary>
    public ICommand Command { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        const int limit = 5;

        var count = Command.Commands.Count;
        var childList = Command.Commands.Take(5).Select(command => command.Name);

        if (count > limit)
        {
            childList = childList.Append($"+ {count - limit} more...");
        }
        
        textWriter.WriteLine($"Command '{Command.Path}' requires a sub command, for example:");

        foreach (var name in childList)
        {
            textWriter.WriteLine($"   {name}");
        }
    }
}