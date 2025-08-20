using Vertical.Cli.Help.Internal;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that formats, aligns, and trims content to be printed by a
/// help text writer. Symbols lists are displayed in compact table format.
/// </summary>
public class CompactLayoutEngine : BaseLayoutEngine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompactLayoutEngine"/> class.
    /// </summary>
    /// <param name="writer">The help text writer to output content to.</param>
    /// <param name="columnWidth">The number of characters that should be printed in a single row without causing
    /// wrapping..</param>
    /// <param name="indentationSpaces">The number of character printed to indicate indentation.</param>
    public CompactLayoutEngine(
        IHelpTextWriter writer, 
        int columnWidth,
        int indentationSpaces = DefaultIndentationWidth)
        : base(writer, columnWidth, indentationSpaces)
    {
    }

    /// <summary>
    /// Writes an identifier list.
    /// </summary>
    /// <param name="listItems">List items to display.</param>
    public override void WriteListItems(HelpListItem[] listItems)
    {
        if (listItems.Length == 0)
            return;
        
        new CompactTableWriter(HelpWriter, ColumnWidth, IndentationSpaces).WriteList(listItems);
    }
}