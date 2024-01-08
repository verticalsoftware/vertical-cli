namespace Vertical.Cli;

/// <summary>
/// Defines scopes for command symbols.
/// </summary>
public enum SymbolScope
{
    /// <summary>
    /// Indicates the symbol is available to the command in which it is defined (default).
    /// </summary>
    Self,
    
    /// <summary>
    /// Indicates the symbol is available to the command in which it is defined and sub commands.
    /// </summary>
    SelfAndDescendents,
    
    /// <summary>
    /// Indicates the symbol is available to sub commands.
    /// </summary>
    Descendents
}