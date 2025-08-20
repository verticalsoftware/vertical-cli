namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the token kind.
/// </summary>
public enum TokenKind
{
    /// <summary>
    /// Indicates an argument value.
    /// </summary>
    ArgumentValue,
    
    /// <summary>
    ///  Indicates an option symbol with or without an accompanying parameter.
    /// </summary>
    OptionSymbol,
    
    /// <summary>
    /// Indicates a directive..
    /// </summary>
    Directive,
    
    /// <summary>
    /// Indicates the double-hyphen terminating token.
    /// </summary>
    Terminator,
    
    /// <summary>
    /// Indicates the value occurred after an argument termination token.
    /// </summary>
    TerminatedValue
}