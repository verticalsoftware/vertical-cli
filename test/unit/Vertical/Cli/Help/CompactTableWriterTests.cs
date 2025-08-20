using System.Text;
using Vertical.Cli.Help.Internal;

namespace Vertical.Cli.Help;

public class CompactTableWriterTests
{
    [Fact]
    public Task WriteList_Formats()
    {
        var item = new HelpListItem("-r, --runtime-identifier",
            "The known .net framework moniker to target (e.g. net6.0, net8.0, net10.0, netstandard2.0, etc). The " +
            "build artifacts will be placed in a directory name that corresponds to the framework.",
            "<RUNTIME_IDENTIFIER>");

        var sb = new StringBuilder(5000);
        var testWidths = new[] { 200, 160, 120, 80, 60, 40 };

        foreach (var width in testWidths)
        {
            sb.Append('-', width);
            sb.AppendLine();
            WriteResult(sb, [item], width);
            sb.AppendLine().AppendLine();
        }

        return Verify(sb.ToString());
    }
    
    private static void WriteResult(
        StringBuilder sb,
        HelpListItem[] items,
        int width)
    {
        using var textWriter = new StringWriter(sb);
        var helpWriter = new NonDecoratedHelpWriter(textWriter);
        var unit = new CompactTableWriter(helpWriter, width, 2);

        unit.WriteList(items);
    }
}