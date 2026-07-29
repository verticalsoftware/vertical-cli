namespace Vertical.Cli.Help;

/// <summary>
/// Defines an interface for objects that can define help content.
/// </summary>
public interface IHelpSubject
{
    /// <summary>
    /// Gets the help topic.
    /// </summary>
    HelpTopic? HelpTopic { get; }
}