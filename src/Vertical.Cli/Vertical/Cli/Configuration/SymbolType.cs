namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the symbol types.
/// </summary>
public enum SymbolType
{
    /// <summary>
    /// Indicates the symbol is an argument.
    /// </summary>
    Argument,
    
    /// <summary>
    /// Indicates the symbol is an option.
    /// </summary>
    Option,
    
    /// <summary>
    /// Indicates the symbol is a switch (<c>Option&lt;bool&gt;).</c>
    /// </summary>
    Switch,
    
    /// <summary>
    /// Indicates a help option.
    /// </summary>
    HelpOption
}