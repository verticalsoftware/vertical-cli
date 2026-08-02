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
    /// Gets extended command remarks.
    /// </summary>
    /// <param name="command">The command instance.</param>
    /// <returns>An array of tuples containing section titles and extended remarks.</returns>
    IEnumerable<CommandExtendedRemarks> GetExtendedRemarks(Command command);

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
    string GetParameterName(IHelpSubject subject);
}