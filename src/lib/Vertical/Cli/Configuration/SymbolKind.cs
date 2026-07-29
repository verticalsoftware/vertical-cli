namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the kinds of symbols.
/// </summary>
public enum SymbolKind
{
    /// <summary>
    /// Indicates a position argument.
    /// </summary>
    PositionArgument,
    
    /// <summary>
    /// Indicates a parameterized option.
    /// </summary>
    Option,
    
    /// <summary>
    /// Indicates a switch.
    /// </summary>
    Switch,
    
    /// <summary>
    /// Indicates a directive symbol.
    /// </summary>
    Directive
}