namespace Vertical.Cli.Help;

/// <summary>
/// Determines where a token filter is display in a help article.
/// </summary>
public enum SymbolGroup
{
    /// <summary>
    /// Indicates the help remarks should be displayed in the arguments section.
    /// </summary>
    Argument,
    
    /// <summary>
    /// Indicates the help remarks should be displayed in the options section.
    /// </summary>
    Options,
    
    /// <summary>
    /// Indicates the help remarks should be displayed in the directives section.
    /// </summary>
    Directives
}