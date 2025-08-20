namespace Vertical.Cli.Help;

/// <summary>
/// Defines help element styles.
/// </summary>
public enum HelpElementKind
{
    /// <summary>
    /// Indicates a section title.
    /// </summary>
    SectionTitle,
    
    /// <summary>
    /// Indicates a command's description.
    /// </summary>
    CommandDescription,
    
    /// <summary>
    /// Indicates the name of a command in a usage syntax clause.
    /// </summary>
    CommandUsageName,
    
    /// <summary>
    /// Indicates an argument token in a usage syntax clause, e.g. &lt;arguments&gt;
    /// </summary>
    ArgumentUsageClauseToken,
    
    /// <summary>
    /// Indicates an option token in a usage syntax clause, e.g. [options]
    /// </summary>
    OptionUsageClauseToken,
    
    /// <summary>
    /// Indicates a named option token in a usage syntax clause, e.g. [options]
    /// </summary>
    NamedOptionUsageClauseToken,
    
    /// <summary>
    /// Indicates a sub command name token in usage syntax clause.
    /// </summary>
    SubCommandNameToken,
    
    /// <summary>
    /// Indicates a token that represents a sub command's arguments and options in usage syntax clause.
    /// </summary>
    SubCommandArgumentsAndOptionsToken,
    
    /// <summary>
    /// Indicates a list item's identifier.
    /// </summary>
    ListItemIdentifier,
    
    /// <summary>
    /// Indicates a list item's parameter syntax.
    /// </summary>
    ListItemParameterSyntax,
    
    /// <summary>
    /// Indicates a list item's description.
    /// </summary>
    ListItemDescription
}