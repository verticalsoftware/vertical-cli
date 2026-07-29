namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the class of an argument's syntax.
/// </summary>
public enum SyntaxKind
{
    /// <summary>
    /// The argument has no symbolic structure.
    /// </summary>
    None,
    
    /// <summary>
    /// The argument is a POSIX or gnu style option. (e.g. -a or --option).
    /// </summary>
    Option,
    
    /// <summary>
    /// The argument is a multi-symbol POSIX option (e.g. -abc)
    /// </summary>
    OptionGroup,
    
    /// <summary>
    /// The argument is a directive (e.g. [key=value].
    /// </summary>
    Directive,
    
    /// <summary>
    /// The argument is an annotation (e.g. @path).
    /// </summary>
    Annotation,
    
    /// <summary>
    /// The argument is the options terminator token <c>--</c>.
    /// </summary>
    OptionsTerminator
}