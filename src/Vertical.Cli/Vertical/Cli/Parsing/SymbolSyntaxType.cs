namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the syntax symbol types.
/// </summary>
public enum SymbolSyntaxType
{
    /// <summary>
    /// Indicates the symbol consists of a non-prefixed word or multi-part word.
    /// </summary>
    Simple,
    
    /// <summary>
    /// Indicates the symbol is prefixed with a single hyphen character.
    /// </summary>
    PosixPrefixed,
    
    /// <summary>
    /// Indicates the symbol is prefixed with two hyphen characters.
    /// </summary>
    GnuPrefixed,
    
    /// <summary>
    /// Indicates the symbol is prefixed with a forward slash.
    /// </summary>
    SlashPrefixed,
    
    /// <summary>
    /// Indicates the symbol is only two hyphens.
    /// </summary>
    ArgumentTerminator,
    
    /// <summary>
    /// Indicates the symbol contains one or more characters that disqualify
    /// it from being an identifier symbol. 
    /// </summary>
    NonIdentifier
}