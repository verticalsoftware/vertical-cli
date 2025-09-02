using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli;

/// <summary>
/// Represents a sub command.
/// </summary>
public sealed class Command : ContainerCommand<EmptyModel>, ISubCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class.
    /// </summary>
    /// <param name="name">The unique name of the command within its set of siblings.</param>
    /// <param name="handler">A handling function that implement's the logic of the application.</param>
    /// <param name="helpTag">An application defined help tag.</param>
    public Command(string name,
        CommandHandler? handler = null,
        CommandHelpTag? helpTag = null) 
        : base(
            name, handler != null ? (_, cancellationToken) => handler(cancellationToken) : null, helpTag)
    {
    }
    
    /// <inheritdoc />
    public ICommand? Parent { get; internal set; }

    private static CommandHandler<EmptyModel>? CreateWrappedHandler(CommandHandler? handler)
    {
        return handler != null
            ? (_, cancellationToken) => handler(cancellationToken)
            : null;
    }
}

/// <summary>
/// Represents a sub command that implements application logic.
/// </summary>
/// <typeparam name="TModel">Options type</typeparam>
public sealed class Command<TModel> : ContainerCommand<TModel>, ISubCommand 
    where TModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Command{T}"/> class.
    /// </summary>
    /// <param name="name">The unique name of the command within its set of siblings.</param>
    /// <param name="handler">A function that receives the option model and returns a result.</param>
    /// <param name="helpTag">An application defined help tag.</param>
    public Command(string name, 
        CommandHandler<TModel> handler, 
        CommandHelpTag? helpTag = null) 
        : base(name, handler, helpTag)
    {
    }

    /// <inheritdoc />
    public ICommand? Parent { get; internal set; }
}