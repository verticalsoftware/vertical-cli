namespace Vertical.Cli.Help;

/// <summary>
/// Defines where help remarks are placed.
/// </summary>
public enum HelpPlacement
{
    /// <summary>
    /// Indicates the content should be placed after the symbol it describes.
    /// </summary>
    Self,
    
    /// <summary>
    /// Indicates the content is placed at the top of help.
    /// </summary>
    Top,
    
    /// <summary>
    /// Indicates the content is placed before the usage section.
    /// </summary>
    BeforeUsage,
    
    /// <summary>
    /// Indicates the content is placed before the commands section.
    /// </summary>
    BeforeCommands,
    
    /// <summary>
    /// Indicates the content is placed before the arguments section.
    /// </summary>
    BeforeArguments,
    
    /// <summary>
    /// Indicates the content is placed before the options section.
    /// </summary>
    BeforeOptions,
    
    /// <summary>
    /// Indicates the content is placed after all other sections.
    /// </summary>
    Bottom
}