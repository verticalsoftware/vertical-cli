using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the sub commands of this instance.
    /// </summary>
    IReadOnlyList<ISubCommand> Commands { get; }
    
    /// <summary>
    /// Gets a space separated path.
    /// </summary>
    string Path { get; }
    
    /// <summary>
    /// Gets the application defined help tag.
    /// </summary>
    CommandHelpTag? HelpTag { get; }
}