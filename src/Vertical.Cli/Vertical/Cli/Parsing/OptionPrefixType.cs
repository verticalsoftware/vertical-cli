namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines option types.
/// </summary>
public enum OptionPrefixType
{
    /// <summary>
    /// Indicates the argument is a value (has no prefix).
    /// </summary>
    None,
    
    /// <summary>
    /// Indicates the argument is a posix style option that begins with a single
    /// dash, is followed by one or more characters, and optionally has an operand.
    /// </summary>
    PosixOption,
    
    /// <summary>
    /// Indicates the argument is a GNU style long option that begins with a
    /// double dash, is followed by an identifier, and optionally has an operand.
    /// </summary>
    GnuOption
}