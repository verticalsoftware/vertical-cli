using System.Text;

namespace Vertical.Cli.Help;

public class LayoutUtilitiesTests
{
    [Fact]
    public Task SplitLines_Returns_Expected()
    {
        const string DummyText = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " + 
                                 "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, " + 
                                 "when an unknown printer took a galley of type and scrambled it to make a type " + 
                                 "specimen book. It has survived not only five centuries, but also the leap into " + 
                                 "electronic typesetting, remaining essentially unchanged. It was popularised in " + 
                                 "the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
                                 "and more recently with desktop publishing software like Aldus PageMaker " +
                                 "including versions of Lorem Ipsum.";

        return Verify(SplitLinesAndJoin(DummyText));
    }
    
    [Fact]
    public Task SplitLines_Returns_Expected_With_Line_Breaks()
    {
        const string DummyText = "Alice\nBetty\nClarissa\nDaphne\nErica";

        return Verify(SplitLinesAndJoin(DummyText));
    }
    
    [Fact]
    public Task SplitLines_Returns_Expected_With_Multi_Line_Breaks()
    {
        const string DummyText = "[Section1]\n    <text>\n\n[Section2]\n    <text>";

        return Verify(SplitLinesAndJoin(DummyText));
    }

    private static string SplitLinesAndJoin(string str)
    {
        var sb = new StringBuilder(2000);
        var split = str.AsSpan().SplitLinesToLength(120);

        for (; !split.IsEmpty; split = split.Right.SplitLinesToLength(120))
        {
            sb.Append(split.Left);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
