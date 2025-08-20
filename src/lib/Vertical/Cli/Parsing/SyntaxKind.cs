namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the format of a <see cref="TokenSyntax"/> instance.
/// </summary>
public enum SyntaxKind
{
    /// <summary>
    /// Indicates the syntax has no other decoration, e.g. a plain string value.
    /// </summary>
    NonDecorated,
    
    /// <summary>
    /// Indicates a symbol prefixed with a dash, two dashes, or a slash.
    /// </summary>
    PrefixedIdentifier,
    
    /// <summary>
    /// Indicates a symbol enclosed in brackets.
    /// </summary>
    EnclosedSymbol,
    
    /// <summary>
    /// Indicates the text is two dash characters.
    /// </summary>
    Terminator
}