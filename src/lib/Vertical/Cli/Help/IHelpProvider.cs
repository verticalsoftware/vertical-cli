namespace Vertical.Cli.Help;

/// <summary>
/// Represents a component of the help system that orchestrates outputting help
/// content to the console.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// Renders help content.
    /// </summary>
    /// <param name="helpModel">The model that contains contextual data about the help subject.</param>
    /// <returns><see cref="Task"/></returns>
    Task RenderHelpAsync(HelpModel helpModel);
}