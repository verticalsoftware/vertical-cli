namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines symbol conventions.
/// </summary>
public enum SymbolConvention
{
    /// <summary>
    /// Indicates a single character symbol prefixed by a hyphen, e.g. -a
    /// </summary>
    PosixOption,
    
    /// <summary>
    /// Indicates a group of characters prefixed by a hyphen, e.g. -abc
    /// </summary>
    PosixGroup,
    
    /// <summary>
    /// Indicates a long form option prefixed by double hyphens, e.g. --option
    /// </summary>
    GnuOption,
    
    /// <summary>
    /// Indicates a Microsoft style option prefixed by a forward slash.
    /// </summary>
    ForwardSlashOption
}