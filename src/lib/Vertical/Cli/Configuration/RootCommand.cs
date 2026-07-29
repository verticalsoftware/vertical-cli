using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the root implementation of the application's function or a pathway to one or more subcommands (or both).
/// </summary>
public sealed class RootCommand : Command
{
    /// <inheritdoc />
    public RootCommand(string name, CommandHelpTopic? helpTopic = null) : base(name, helpTopic)
    {
    }
}