using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents a list element in a help article's symbol table.
/// </summary>
public interface IListElement
{
    /// <summary>
    /// Gets the remarks.
    /// </summary>
    string Remarks { get; }
    
    /// <summary>
    /// Gets the computed width of the element.
    /// </summary>
    int ComputedWidth { get; }
    
    /// <summary>
    /// Renders the element to the given writer.
    /// </summary>
    /// <param name="writer">The output writer.</param>
    void RenderSyntax(OutputWriter writer);
}