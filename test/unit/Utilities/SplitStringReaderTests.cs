using System.Text;
using Shouldly;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.UnitTests.Utilities;

public class SplitStringReaderTests
{
    private const int SplitLength = 80;

    private const string SingleLineInput =
        "Unit testing is crucial as it ensures that individual components of a software application function correctly. By validating each unit of code, developers can identify and fix bugs early in the development process, which ultimately leads to higher quality software and reduced costs associated with later-stage debugging.";
    
    private const string MultiLineInput =
        """
        Unit testing is crucial as it ensures that individual components of a software application function correctly. By validating each unit of code, developers can identify and fix bugs early in the development process, which ultimately leads to higher quality software and reduced costs associated with later-stage debugging.
        
        Additionally, unit tests serve as a form of documentation, providing clear examples of how specific functions are intended to work. This not only aids in maintaining the code but also facilitates collaboration among team members, as it allows new developers to understand the codebase more quickly and effectively.
        """;
    
    [Fact]
    public void SplitNothing_Returns_Nothing()
    {
        var unit = new SplitStringReader(string.Empty, SplitLength);
        
        unit.TryReadLine(out _).ShouldBeFalse();
    }

    [Fact]
    public void Split_Small_With_No_WhiteSpace_Concerns_Yields_Input()
    {
        var unit = new SplitStringReader("This is a simple sentence.", SplitLength);
        
        unit.TryReadLine(out var span).ShouldBeTrue();
        span.ToString().ShouldBe("This is a simple sentence.");
        unit.TryReadLine(out _).ShouldBeFalse();
    }
    
    [Fact]
    public void Split_Small_With_Leading__WhiteSpace_Concerns_Yields_Input()
    {
        var unit = new SplitStringReader("   This is a simple sentence.   ", SplitLength);
        
        unit.TryReadLine(out var span).ShouldBeTrue();
        span.ToString().ShouldBe("This is a simple sentence.");
        unit.TryReadLine(out _).ShouldBeFalse();
    }

    [Fact]
    public Task Split_Big_Single_Line_Yields_Correct()
    {
        return Verify(Collect(SingleLineInput));
    }

    [Fact]
    public Task Split_Big_Multi_Line_Yields_Correct()
    {
        return Verify(Collect(MultiLineInput));
    }

    private static string Collect(string str)
    {
        var sb = new StringBuilder();
        var unit = new SplitStringReader(str, SplitLength);

        while (unit.TryReadLine(out var span))
        {
            sb.Append(span);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}