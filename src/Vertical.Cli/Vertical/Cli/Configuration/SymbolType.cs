namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the types of symbols definable in a command.
/// </summary>
public enum SymbolType
{
    /// <summary>
    /// Indicates a positional argument symbol.
    /// </summary>
    Argument,
    
    /// <summary>
    /// Indicates an option symbol.
    /// </summary>
    Option,
    
    /// <summary>
    /// Indicates a switch symbol.
    /// </summary>
    Switch,
    
    /// <summary>
    /// Indicates an action.
    /// </summary>
    Action
}