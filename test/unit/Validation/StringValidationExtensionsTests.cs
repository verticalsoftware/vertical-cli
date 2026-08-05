using System.Text.RegularExpressions;
using NSubstitute;
using Vertical.Cli.Validation;

namespace Vertical.Cli.UnitTests.Validation;

public class StringValidationExtensionsTests
{
    [Fact]
    public void MustBeOfLength_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create("hello");
        eventInfo.MustBeOfLength(5);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOfLength_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create("hi");
        eventInfo.MustBeOfLength(5);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustNotExceedLength_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create("hello");
        eventInfo.MustNotExceedLength(10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustNotExceedLength_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create("hello world");
        eventInfo.MustNotExceedLength(5);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPattern_String_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create("abc123");
        eventInfo.MustMatchPattern(@"^[a-z]+\d+$");
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPattern_String_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create("123abc");
        eventInfo.MustMatchPattern(@"^[a-z]+\d+$");
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPattern_Regex_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create("abc123");
        eventInfo.MustMatchPattern(new Regex(@"^[a-z]+\d+$"));
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPattern_Regex_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create("123abc");
        eventInfo.MustMatchPattern(new Regex(@"^[a-z]+\d+$"));
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOfLengthOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>("hello");
        eventInfo.MustBeOfLengthOrBeNull(5);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOfLengthOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>(null);
        eventInfo.MustBeOfLengthOrBeNull(5);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOfLengthOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<string?>("hi");
        eventInfo.MustBeOfLengthOrBeNull(5);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustNotExceedLengthOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>("hello");
        eventInfo.MustNotExceedLengthOrBeNull(10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustNotExceedLengthOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>(null);
        eventInfo.MustNotExceedLengthOrBeNull(10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustNotExceedLengthOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<string?>("hello world");
        eventInfo.MustNotExceedLengthOrBeNull(5);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPatternOrBeNull_String_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>("abc123");
        eventInfo.MustMatchPatternOrBeNull(@"^[a-z]+\d+$");
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPatternOrBeNull_String_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>(null);
        eventInfo.MustMatchPatternOrBeNull(@"^[a-z]+\d+$");
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPatternOrBeNull_String_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<string?>("123abc");
        eventInfo.MustMatchPatternOrBeNull(@"^[a-z]+\d+$");
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPatternOrBeNull_Regex_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>("abc123");
        eventInfo.MustMatchPatternOrBeNull(new Regex(@"^[a-z]+\d+$"));
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPatternOrBeNull_Regex_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string?>(null);
        eventInfo.MustMatchPatternOrBeNull(new Regex(@"^[a-z]+\d+$"));
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustMatchPatternOrBeNull_Regex_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<string?>("123abc");
        eventInfo.MustMatchPatternOrBeNull(new Regex(@"^[a-z]+\d+$"));
        eventInfo.Received().Error(Arg.Any<string>());
    }
}
