using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli;

/// <summary>
/// Represents a sub command.
/// </summary>
public sealed class RootCommand : ContainerCommand<EmptyModel>, IRootCommand
{
    /// <inheritdoc />
    public RootCommand(
        string name,
        CommandHandler? handler = null,
        CommandHelpTag? helpTag = null) 
        : base(
            name, handler != null ? (_, cancellationToken) => handler(cancellationToken) : null, helpTag)
    {
    }
}

/// <summary>
/// Represents an invokable root command.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class RootCommand<TModel> : ContainerCommand<TModel>, IRootCommand where TModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootCommand{T}"/> class.
    /// </summary>
    /// <param name="name">The unique name of the command within its set of siblings.</param>
    /// <param name="handler">A function that receives the option model and returns a result.</param>
    /// <param name="helpTag">A help tag to associate with the command.</param>
    public RootCommand(string name, CommandHandler<TModel> handler, CommandHelpTag? helpTag = null) 
        : base(name, handler, helpTag)
    {
    }
}