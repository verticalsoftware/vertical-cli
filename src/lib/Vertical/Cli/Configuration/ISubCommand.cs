namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a sub command.
/// </summary>
public interface ISubCommand : ICommand
{
    /// <summary>
    /// Gets the sub-command's parent reference.
    /// </summary>
    ICommand? Parent { get; }
}