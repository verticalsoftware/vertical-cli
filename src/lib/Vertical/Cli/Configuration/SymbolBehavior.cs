namespace Vertical.Cli.Configuration;

/// <summary>
/// Describes the behavior of a symbol.
/// </summary>
public enum SymbolBehavior
{
    /// <summary>
    /// Indicates arguments from the command line will be paired to the symbol
    /// after other symbol types have been parsed.
    /// </summary>
    Argument,
    
    /// <summary>
    /// Indicates a symbol that has one or more prefixed identifiers and requires an
    /// attached or accompanied parameter value that is bound to a model property.
    /// </summary>
    Option,
    
    /// <summary>
    /// Indicates a symbol that has one or more prefixed identifiers that is bound to
    /// a <c>bool</c> model property. In the absence of an explicit parameter value,
    /// <c>true</c> is inferred.
    /// </summary>
    Switch
}