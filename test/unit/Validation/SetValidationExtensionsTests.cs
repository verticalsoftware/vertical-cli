using NSubstitute;
using Vertical.Cli.Validation;

namespace Vertical.Cli.UnitTests.Validation;

public class SetValidationExtensionsTests
{
    [Fact]
    public void MustBeOneOf_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(2);
        eventInfo.MustBeOneOf(new HashSet<int> { 1, 2, 3 });
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOneOf_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(4);
        eventInfo.MustBeOneOf(new HashSet<int> { 1, 2, 3 });
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void CannotBeOneOf_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(4);
        eventInfo.CannotBeOneOf(new HashSet<int> { 1, 2, 3 });
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void CannotBeOneOf_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(2);
        eventInfo.CannotBeOneOf(new HashSet<int> { 1, 2, 3 });
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOneOfOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(2);
        eventInfo.MustBeOneOfOrBeNull(new HashSet<int?> { 1, 2, 3 });
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOneOfOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeOneOfOrBeNull(new HashSet<int?> { null, 1, 2, 3 });
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeOneOfOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(4);
        eventInfo.MustBeOneOfOrBeNull(new HashSet<int?> { 1, 2, 3 });
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void CannotBeOneOfOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(4);
        eventInfo.CannotBeOneOfOrBeNull(new HashSet<int?> { 1, 2, 3 });
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void CannotBeOneOfOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.CannotBeOneOfOrBeNull(new HashSet<int?> { 1, 2, 3 });
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void CannotBeOneOfOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(2);
        eventInfo.CannotBeOneOfOrBeNull(new HashSet<int?> { 1, 2, 3 });
        eventInfo.Received().Error(Arg.Any<string>());
    }
}
