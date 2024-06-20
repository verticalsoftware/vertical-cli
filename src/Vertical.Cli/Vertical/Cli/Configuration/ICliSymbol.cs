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
}