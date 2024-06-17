using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that provides help content.
/// </summary>
public interface IHelpContentProvider
{
    /// <summary>
    /// Gets the content for a Cli object.
    /// </summary>
    /// <param name="cliObject">Command or symbol reference.</param>
    /// <returns>Description of the object or <c>null</c>.</returns>
    string? GetContent(CliPrimitive cliObject);
}