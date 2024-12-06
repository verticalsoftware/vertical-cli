namespace Vertical.Cli.Help;

/// <summary>
/// Composes help content and outputs the result to a text writer.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// When implemented, composes help content.
    /// </summary>
    /// <param name="context">A context that contains information about the help topic.</param>
    /// <returns>Task</returns>
    Task WriteContentAsync(HelpContext context);
}