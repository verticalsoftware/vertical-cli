using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the context for displaying help.
/// </summary>
/// <param name="Command">The command that help is being requested for.</param>
/// <param name="HelpProvider">The help provider.</param>
public record HelpContext(CliCommand Command, IHelpProvider HelpProvider);
    