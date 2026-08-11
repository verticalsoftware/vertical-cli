namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the scope for an unbound symbol.
/// </summary>
public enum UnboundScope
{
    /// <summary>
    /// Indicates an unbound symbol is locally scoped.
    /// </summary>
    Local,
    
    /// <summary>
    /// Indicates an unbound symbol is globally scoped.
    /// </summary>
    Global
}