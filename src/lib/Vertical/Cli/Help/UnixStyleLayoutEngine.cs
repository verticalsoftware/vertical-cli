namespace Vertical.Cli.Help;

/// <summary>
/// Represents an object that formats, aligns, and trims content to be printed by a
/// help text writer. Symbols lists in indented paragraph format.
/// </summary>
public sealed class UnixStyleLayoutEngine : BaseLayoutEngine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnixStyleLayoutEngine"/> class.
    /// </summary>
    /// <param name="writer">The help text writer to output content to.</param>
    /// <param name="columnWidth">The number of characters that should be printed in a single row without causing
    /// wrapping..</param>
    /// <param name="indentationSpaces">The number of character printed to indicate indentation.</param>
    public UnixStyleLayoutEngine(IHelpTextWriter writer, 
        int columnWidth, 
        int indentationSpaces = ManPageDefaultIndentationWidth) 
        : base(writer, columnWidth, indentationSpaces)
    {
    }

    /// <summary>
    /// Defines the default indentation width.
    /// </summary>
    public const int ManPageDefaultIndentationWidth = 4;

    /// <inheritdoc />
    public override void WriteListItems(HelpListItem[] listItems)
    {
        var appendLine = false;
        
        foreach (var item in listItems)
        {
            if (appendLine)
            {
                WriteLine();
            }
            
            WriteIndentation();
            HelpWriter.WriteElement(HelpElementKind.ListItemIdentifier, item.Identifier);

            if (item.ParameterSyntax is { Length: > 0 })
            {
                HelpWriter.WriteWhiteSpace(1);
                HelpWriter.WriteElement(HelpElementKind.ListItemParameterSyntax, item.ParameterSyntax);
            }

            WriteLine();

            if (item.Description is { Length: > 0 })
            {
                WriteParagraph(HelpElementKind.ListItemDescription, item.Description, 2);
            }

            appendLine = true;
        }
    }
}