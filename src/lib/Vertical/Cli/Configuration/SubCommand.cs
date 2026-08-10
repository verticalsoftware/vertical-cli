using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an implementation of an application function or a pathway to one or more subcommands (or both).
/// </summary>
public sealed class SubCommand : Command
{
    /// <inheritdoc />
    public SubCommand(CommandName name, CommandHelpTopic? helpTopic = null) : base(name.Value, helpTopic)
    {
    }
}