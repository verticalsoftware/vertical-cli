using Vertical.Cli.Help;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Abstract type for parent commands.
/// </summary>
public abstract class ContainerCommand<TModel> : ICommand where TModel : class
{
    internal ContainerCommand(string name,
        CommandHandler<TModel>? handler,
        CommandHelpTag? helpTag)
    {
        _handler = handler;
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Name = name;
        HelpTag = helpTag;
    }

    private readonly CommandHandler<TModel>? _handler;

    private readonly List<ISubCommand> _subCommands = [];

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    public IReadOnlyList<ISubCommand> Commands => _subCommands;

    /// <inheritdoc />
    public string Path => Parent is { } parent
        ? $"{parent.Path} {Name}"
        : Name;

    /// <inheritdoc />
    public CommandHelpTag? HelpTag { get; }

    /// <inheritdoc />
    public bool IsInvocationTarget => _handler != null;

    /// <inheritdoc />
    public HandlerContextBuilder CreateRequestBuilder(
        IRootConfiguration configuration,
        IModelConfigurationFactory modelConfigurationFactory)
    {
        if (_handler == null)
        {
            throw Exceptions.InvalidCommandInvocationPath(this);
        }

        var requestBuilder = new HandlerContextBuilder<TModel>(
            this,
            _handler);

        return configuration.ConfigureRequestBuilder(
            requestBuilder,
            typeof(TModel),
            modelConfigurationFactory);
    }
    
    

    /// <summary>
    /// Adds a sub command.
    /// </summary>
    /// <param name="command">The command that will become a child of this instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="command"></paramref> is null</exception>
    /// <exception cref="ArgumentException"><paramref name="command"></paramref> has a name that is already in use</exception>
    public void AddCommand(Command command)
    {
        if (_subCommands.Any(child => child.Name.Equals(command.Name)))
        {
            throw new ArgumentException($"Sub command '{command.Name}' already added");
        }
        
        _subCommands.Add(command ?? throw new ArgumentNullException(nameof(command)));
        command.Parent = this;
    }

    /// <summary>
    /// Adds a sub command.
    /// </summary>
    /// <param name="command">The command that will become a child of this instance.</param>
    /// <typeparam name="TChildModel">The command's options model type</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="command"></paramref> is null</exception>
    public void AddCommand<TChildModel>(Command<TChildModel> command) where TChildModel : class
    {
        if (_subCommands.Any(child => child.Name.Equals(command.Name)))
        {
            throw new ArgumentException($"Sub command '{command.Name}' already added");
        }
        
        _subCommands.Add(command ?? throw new ArgumentNullException(nameof(command)));
        command.Parent = this;
    }

    private ICommand? Parent => (this as ISubCommand)?.Parent;

    /// <inheritdoc />
    public override string ToString() => Path;
}