using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Renders help content.
/// </summary>
public interface IHelpRenderer
{
    /// <summary>
    /// Writes help content for the given command.
    /// </summary>
    /// <param name="command">The command for which documentation is being rendered.</param>
    void WriteContent(ICommandDefinition command);
}