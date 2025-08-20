using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help.Internal;

internal sealed class CompactTableWriter(
    IHelpTextWriter textWriter, 
    int columnWidth,
    int indentationSpaces)
{
    private const int ColumnSeparationSpaces = 3;
    
    public void WriteList(HelpListItem[] listItems)
    {
        var adjustedWidth = columnWidth - indentationSpaces;
        var maxDescriptionLength = listItems.Max(item => item.Description?.Length ?? 0);

        if (maxDescriptionLength == 0)
        {
            WriteListItems(listItems, (adjustedWidth, 0));
        }

        var identifierColumnWidth = ComputeIdentifierColumnWidth(listItems)
                                    + ColumnSeparationSpaces
                                    + indentationSpaces;

        var descriptionColumnWidth = adjustedWidth - identifierColumnWidth;
        
        WriteListItems(listItems, (identifierColumnWidth, descriptionColumnWidth));
    }

    private void WriteListItems(HelpListItem[] listItems, (int Left, int Right) columnWidths)
    {
        foreach (var item in listItems)
        {
            WriteListItem(item, columnWidths);
        }
    }

    private void WriteListItem(HelpListItem item, (int Left, int Right) columnWidths)
    {
        var leftSplit = item.Identifier.SplitLinesToLength(columnWidths.Left);
        var leftSplitKind = HelpElementKind.ListItemIdentifier;
        var rightSplit = item.Description != null
            ? item.Description.SplitLinesToLength(columnWidths.Right)
            : SpanPair.Empty;

        while (!(leftSplit.IsEmpty && rightSplit.IsEmpty))
        {
            WriteIdentifierAndParameter(
                item,
                ref leftSplit,
                ref leftSplitKind,
                columnWidths.Left);

            if (rightSplit.Left.Length > 0)
            {
                textWriter.WriteElement(HelpElementKind.ListItemDescription, rightSplit.Left);
                rightSplit = rightSplit.ResplitLinesToLength(columnWidths.Right);
            }

            textWriter.WriteLine();
        }
    }

    private void WriteIdentifierAndParameter(HelpListItem item, ref SpanPair split, ref HelpElementKind kind, int width)
    {
        var position = 0;

        try
        {
            if (split.Left.Length == 0)
                return;
            
            textWriter.WriteWhiteSpace(indentationSpaces);
            textWriter.WriteElement(kind, split.Left);
            
            position = indentationSpaces + split.Left.Length;
            split = split.ResplitLinesToLength(width);

            if (split.Left.Length > 0)
            {
                // Still remaining text to write for the current element
                return;
            }

            if (kind == HelpElementKind.ListItemParameterSyntax)
            {
                // We processed both components
                return;
            }

            kind = HelpElementKind.ListItemParameterSyntax;

            if (item.ParameterSyntax is not { Length: > 0 })
            {
                // There is no parameter to print
                return;
            }

            var remainingWidth = width - position - 1; // need to space

            if (item.ParameterSyntax.Length <= remainingWidth)
            {
                // Can fit parameter on same line
                textWriter.WriteWhiteSpace(1);
                textWriter.WriteElement(kind, item.ParameterSyntax);
                split = SpanPair.Empty;
                position += item.ParameterSyntax.Length + 1;
                return;
            }
            
            // Wrap
            split = item.ParameterSyntax.SplitLinesToLength(width);
        }
        finally
        {
            textWriter.WriteWhiteSpace(width - position);
        }
    }

    private int ComputeIdentifierColumnWidth(HelpListItem[] items)
    {
        var availableWidth = items.Any(item => item is { Description.Length: > 0 })
            ? columnWidth / 2 - indentationSpaces
            : columnWidth - indentationSpaces;
        
        var measurements = items
            .Select(item => (IdWidth: item.Identifier.Length, ParamWidth: item.ParameterSyntax?.Length + 1 ?? 0))
            .ToArray();

        var maxFullWidth = measurements.Max(m => m.IdWidth + m.ParamWidth);
        if (maxFullWidth <= availableWidth)
            return maxFullWidth;

        var maxSeparatedWidth = Math.Max(
            measurements.Max(m => m.IdWidth),
            measurements.Max(m => m.ParamWidth));

        return maxSeparatedWidth <= availableWidth ? maxSeparatedWidth : availableWidth;
    }
}