namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the token kind.
/// </summary>
public enum TokenKind
{
    /// <summary>
    /// Indicates the token represents command name or an argument.
    /// </summary>
    CommandOrArgument,
    
    /// <summary>
    /// Indicates the token represents a positional argument.
    /// </summary>
    Argument,
    
    /// <summary>
    /// Indicates the token represents an option or switch.
    /// </summary>
    Option,
    
    /// <summary>
    /// Indicates the token represents the options terminator, e.g, <c>--</c>.
    /// </summary>
    OptionsTerminator,
    
    /// <summary>
    /// Indicates the token represents a directive.
    /// </summary>
    Directive,
    
    /// <summary>
    /// Indicates the token represents an annotation.
    /// </summary>
    Annotation
}