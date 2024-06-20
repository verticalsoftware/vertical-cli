// ReSharper disable once CheckNamespace
namespace Vertical.Cli;

/// <summary>
/// Defines the scope of a symbol.
/// </summary>
public enum CliScope
{
    /// <summary>
    /// Indicates the symbol applies only to the command in which it is defined.
    /// </summary>
    Self,
    
    /// <summary>
    /// Indicates the symbol applies to sub-commands of the command in which it is
    /// defined.
    /// </summary>
    Descendants,
    
    /// <summary>
    /// Indicates the symbol applies to sub-commands and the command in which it is
    /// defined.
    /// </summary>
    SelfAndDescendants
}