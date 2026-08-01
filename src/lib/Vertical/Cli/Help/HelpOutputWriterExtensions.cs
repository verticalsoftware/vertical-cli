using Vertical.Cli.IO;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

internal readonly record struct LineBounds(int Left, int Right)
{
    public int Width => Right - Left;

    public static LineBounds RightJustified(int left, int width) => new(left, width - left);
}

internal static class HelpOutputWriterExtensions
{
    extension(OutputWriter writer)
    {
        public void WriteParagraph(string content, LineBounds lineBounds, DisplayElement? displayElement = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;
            
            var width = lineBounds.Width;
            
            writer.SetDisplayElement(displayElement ?? DisplayElement.Default);

            var contentSpan = content.AsSpan();
            
            foreach (var range in contentSpan.Split(Environment.NewLine))
            {
                var subSpan = contentSpan[range];
                var split = new SplitSpan(subSpan).SplitToWidth(width);
                
                for(; split.HasSlice; split = split.SplitToWidth(width))
                {
                    writer.WriteWhiteSpace(lineBounds.Left);
                    writer.WriteLine(split.Slice);
                }
            }
        }
        
        public void WriteTable<T>(
            IEnumerable<T> entries,
            Func<T, int> measureColumn1,
            Action<T> renderColumn1,
            Func<T, string?> getColumn2,
            DisplayElement? column2Element,
            LineBounds lineBounds,
            int indentSpaces)
        {
            var entryArray = entries.ToArray();
            if (entryArray.Length == 0) return;

            const int spacing = 4;
            var width = lineBounds.Width;
            var column1CellWidths = entryArray.Select(measureColumn1).ToArray();
            var column1Width = column1CellWidths.Max();
            var column2Width = width - column1Width - spacing;

            if (column2Width < width * .5)
            {
                writer.WriteTableUsingNixLayout(
                    entryArray, 
                    renderColumn1, 
                    getColumn2, 
                    column2Element, 
                    lineBounds,
                    indentSpaces);
                return;
            }

            var column2Left = lineBounds.Left + column1Width + spacing;
            var column2LineBounds = new LineBounds(column2Left, column2Left + column2Width);

            writer.Return();
            
            for (var c = 0; c < entryArray.Length; c++)
            {
                var entry = entryArray[c];
                writer.WriteWhiteSpace(lineBounds.Left);
                renderColumn1(entry);

                if (getColumn2(entry) is not { Length: > 0 } column2Content)
                {
                    writer.Return();
                    continue;
                }
                
                var span = new SplitSpan(column2Content).SplitToWidth(column2LineBounds.Width);
                var lineId = 0;
                
                for (; span.HasSlice; span = span.SplitToWidth(column2LineBounds.Width))
                {
                    var wsCount = lineId++ == 0
                        ? column2LineBounds.Left - column1CellWidths[c]
                        : column2LineBounds.Left + lineBounds.Left;
                    
                    writer.WriteWhiteSpace(wsCount);
                    writer.Write(span.Slice, column2Element);
                    writer.WriteLine();
                }
                
                writer.Return();
            }
        }
        
        public void WriteTable<T>(
            IEnumerable<T> entries,
            LineBounds lineBounds)
            where T : IListElement
        {
            var entryArray = entries.ToArray();
            if (entryArray.Length == 0) return;

            const int spacing = 4;
            var width = lineBounds.Width;
            var column1CellWidths = entryArray
                .Select(element => element.ComputedWidth + FormattingConstants.ColumnSeparatorWidth)
                .ToArray();
            var column1Width = column1CellWidths.Max();
            var column2Width = width - column1Width - spacing;

            if (column2Width < width * .5)
            {
                return;
            }

            var column2Left = lineBounds.Left + column1Width + spacing;
            var column2LineBounds = new LineBounds(column2Left, column2Left + column2Width);

            writer.Return();
            
            for (var c = 0; c < entryArray.Length; c++)
            {
                var entry = entryArray[c];
                writer.WriteWhiteSpace(lineBounds.Left);
                entry.RenderSyntax(writer);

                if (entry.Remarks is not { Length: > 0 } column2Content)
                {
                    writer.Return();
                    continue;
                }
                
                var span = new SplitSpan(column2Content).SplitToWidth(column2LineBounds.Width);
                var lineId = 0;
                
                for (; span.HasSlice; span = span.SplitToWidth(column2LineBounds.Width))
                {
                    var wsCount = lineId++ == 0
                        ? column2LineBounds.Left - column1CellWidths[c]
                        : column2LineBounds.Left + lineBounds.Left - FormattingConstants.ColumnSeparatorWidth;
                    
                    writer.WriteWhiteSpace(wsCount);
                    writer.Write(span.Slice, DisplayElement.Remarks);
                    writer.WriteLine();
                }
                
                writer.Return();
            }
        }

        private void WriteTableUsingNixLayout<T>(
            IEnumerable<T> entries,
            Action<T> renderColumn1,
            Func<T, string?> getColumn2,
            DisplayElement? column2Element,
            LineBounds lineBounds,
            int indentSpaces)
        {
            var indentedLineBounds = new LineBounds(
                lineBounds.Left + indentSpaces,
                lineBounds.Right - indentSpaces);
            
            foreach (var entry in entries)
            {
                renderColumn1(entry);
                writer.WriteLine();

                if (getColumn2(entry) is { Length: > 0 } column2Content)
                {
                    writer.WriteParagraph(column2Content, indentedLineBounds, column2Element);
                }

                writer.WriteLine();
            }
        }
    }
}