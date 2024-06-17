using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that formats help content.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// When implemented by a class, creates help content for the given command.
    /// </summary>
    /// <param name="command">Command that help is being displayed for.</param>
    /// <returns><see cref="string"/></returns>
    string GetContent(CliCommand command);
}