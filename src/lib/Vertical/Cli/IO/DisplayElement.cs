namespace Vertical.Cli.IO;

/// <summary>
/// Defines the display elements output to the console abstraction.
/// </summary>
public enum DisplayElement
{
    /// <summary>
    /// Indicates the default element.
    /// </summary>
    Default,
    
    /// <summary>
    /// Indicates an error message or other important text.
    /// </summary>
    Important,
    
    /// <summary>
    /// Indicates a section heading.
    /// </summary>
    Heading,
    
    /// <summary>
    /// Indicates a general remark.
    /// </summary>
    Remarks,
    
    /// <summary>
    /// Indicates the name of a command
    /// </summary>
    CommandName,
    
    /// <summary>
    /// Indicates a usage syntax
    /// </summary>
    UsageSyntax,
    
    /// <summary>
    /// Indicates an identifier in a list.
    /// </summary>
    ListIdentifier,
    
    /// <summary>
    /// Indicates a parameter syntax
    /// </summary>
    ParameterSyntax,
    
    /// <summary>
    /// Indicates the required arity annotation.
    /// </summary>
    RequiredAnnotation
}