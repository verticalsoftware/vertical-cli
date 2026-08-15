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
    
    /// <summary>
    /// Gets the help topic key for this instance.
    /// </summary>
    HelpTopicKey HelpTopicKey { get; }
    
    /// <summary>
    /// Gets helps remarks for the subject.
    /// </summary>
    /// <returns>String, or <c>null</c> if there are no remarks.</returns>
    string? GetRemarks();

    /// <summary>
    /// Gets extended help remarks.
    /// </summary>
    /// <returns>An enumeration of <see cref="ExtendedRemarksSection"/></returns>
    IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections();

    /// <summary>
    /// Gets an identifier used in help topic lists.
    /// </summary>
    /// <returns><see cref="string"/></returns>
    string GetListIdentifier();

    /// <summary>
    /// If the subject supports a parameter, gets the parameter name.
    /// </summary>
    /// <returns>Parameter name of <c>null</c>.</returns>
    string? GetParameterName();
}