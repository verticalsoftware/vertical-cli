namespace Vertical.Cli;

/// <summary>
/// Defines scopes for command symbols.
/// </summary>
public enum SymbolScope
{
    /// <summary>
    /// Indicates the symbol is available to the command in which it is defined (default).
    /// </summary>
    Parent,
    
    /// <summary>
    /// Indicates the symbol is available to the command in which it is defined and sub commands.
    /// </summary>
    ParentAndDescendents,
    
    /// <summary>
    /// Indicates the symbol is available to sub commands.
    /// </summary>
    Descendents
}