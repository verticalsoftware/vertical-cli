using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that provides help content.
/// </summary>
public interface IHelpProvider
{
    /// <summary>
    /// Gets the help remarks for the given subject.
    /// </summary>
    /// <param name="subject">The subject instance.</param>
    /// <returns>Help content or <c>null</c>.</returns>
    string? GetRemarks(IHelpSubject subject);

    /// <summary>
    /// Gets the number of content sections for a command.
    /// </summary>
    /// <param name="command">The command instance.</param>
    /// <returns><see cref="int"/></returns>
    int GetCommandSectionsCount(Command command);

    /// <summary>
    /// Gets a command section heading.
    /// </summary>
    /// <param name="command">The command instance that contains the section.</param>
    /// <param name="sectionId">The zero based section id.</param>
    /// <returns>The section heading.</returns>
    string GetCommandSectionHeading(Command command, int sectionId);
    
    /// <summary>
    /// Gets a command section remarks.
    /// </summary>
    /// <param name="command">The command instance that contains the section.</param>
    /// <param name="sectionId">The zero based section id.</param>
    /// <returns>The section heading.</returns>
    string GetCommandSectionRemarks(Command command, int sectionId);

    /// <summary>
    /// Gets an identifier for a argument, option, or switch symbol.
    /// </summary>
    /// <param name="subject">The symbol instance.</param>
    /// <returns><see cref="string"/></returns>
    string GetListIdentifier(IHelpSubject subject);

    /// <summary>
    /// Gets the syntax of a symbol's parameter.
    /// </summary>
    /// <param name="subject">The symbol or directive instance.</param>
    /// <returns><see cref="string"/></returns>
    string GetParameterValueSyntax(IHelpSubject subject);
}