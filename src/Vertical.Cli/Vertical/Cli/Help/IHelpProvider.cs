using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that formats help content.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// Renders the help content to the console.
    /// </summary>
    /// <param name="command">Command that help is being displayed for.</param>
    /// <param name="options"></param>
    void RenderContent(CliCommand command, CliOptions options);
}