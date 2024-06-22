namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a renderable help option.
/// </summary>
public interface ICliSymbol
{
    /// <summary>
    /// Gets the symbol names.
    /// </summary>
    string[] Names { get; }
    
    /// <summary>
    /// Gets the primary identifier.
    /// </summary>
    string PrimaryIdentifier { get; }
    
    /// <summary>
    /// Gets the description.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// Gets the operand syntax to display in help content.
    /// </summary>
    string? OperandSyntax { get; }
    
    /// <summary>
    /// Gets the index of the item.
    /// </summary>
    int Index { get; }
    
    /// <summary>
    /// Gets the parent symbol.
    /// </summary>
    ICliSymbol? ParentSymbol { get; }
}