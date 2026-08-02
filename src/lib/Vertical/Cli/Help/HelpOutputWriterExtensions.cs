using Vertical.Cli.IO;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

internal readonly record struct LineBounds(int Left, int Right)
{
    public int Width => Right - Left;

    public static LineBounds RightJustified(int left, int width) => new(left, width - left);

    public LineBounds Offset(int count) => new(Left + count, Right - count);
}

internal static class HelpOutputWriterExtensions
{
    extension(OutputWriter writer)
    {
        public void WriteParagraph(string content, LineBounds bounds, DisplayElement? displayElement = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            writer.SetDisplayElement(displayElement ?? DisplayElement.Default);
            
            var reader = new SplitStringReader(content, bounds.Width);
            while (reader.TryReadLine(out var lineSpan))
            {
                writer.WriteWhiteSpace(bounds.Left);
                writer.WriteLine(lineSpan);
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
                writer.WriteTableManPageStyle(entryArray, lineBounds);
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

                var reader = new SplitStringReader(column2Content, column2LineBounds.Width);
                var lineId = 0;

                while (reader.TryReadLine(out var lineSpan))
                {
                    var wsCount = lineId++ == 0
                        ? column2LineBounds.Left - column1CellWidths[c]
                        : column2LineBounds.Left + lineBounds.Left - FormattingConstants.ColumnSeparatorWidth;
                    
                    writer.WriteWhiteSpace(wsCount);
                    writer.Write(lineSpan, DisplayElement.Remarks);
                    writer.WriteLine();
                }
                
                writer.Return();
            }
        }

        private void WriteTableManPageStyle<T>(T[] entries, LineBounds lineBounds) where T : IListElement
        {
            var remarksBounds = lineBounds.Offset(FormattingConstants.IndentSpaces * 2);
                
            foreach (var entry in entries)
            {
                writer.WriteWhiteSpace(lineBounds.Left);
                entry.RenderSyntax(writer);
                writer.WriteLine();
                writer.WriteParagraph(entry.Remarks, remarksBounds, DisplayElement.Remarks);
                writer.WriteLine();
            }
        }
    }
}