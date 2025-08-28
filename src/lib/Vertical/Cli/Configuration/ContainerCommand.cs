using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Abstract type for parent commands.
/// </summary>
public abstract class ContainerCommand : ICommand
{
    internal ContainerCommand(string name, CommandHelpTag? helpTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Name = name;
        HelpTag = helpTag;
    }

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
    /// <typeparam name="TModel">The command's options model type</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="command"></paramref> is null</exception>
    public void AddCommand<TModel>(Command<TModel> command) where TModel : class
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