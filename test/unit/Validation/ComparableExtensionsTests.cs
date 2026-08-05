using NSubstitute;
using Vertical.Cli.Validation;

namespace Vertical.Cli.UnitTests.Validation;

public class ComparableExtensionsTests
{
    [Fact]
    public void MustBeLessThan_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(0);
        eventInfo.MustBeLessOrEqualTo(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }
    
    [Fact]
    public void MustBeLessThan_With_Invalid_Valid_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(2);
        eventInfo.MustBeLessOrEqualTo(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessThan_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(1);
        eventInfo.MustBeLessThan(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterThan_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(2);
        eventInfo.MustBeGreaterThan(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterThan_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(1);
        eventInfo.MustBeGreaterThan(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterOrEqualTo_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(1);
        eventInfo.MustBeGreaterOrEqualTo(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterOrEqualTo_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(0);
        eventInfo.MustBeGreaterOrEqualTo(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheInclusiveRangeOf_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(5);
        eventInfo.MustBeInTheInclusiveRangeOf(1, 10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheInclusiveRangeOf_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(11);
        eventInfo.MustBeInTheInclusiveRangeOf(1, 10);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheExclusiveRangeOf_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create(5);
        eventInfo.MustBeInTheExclusiveRangeOf(1, 10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheExclusiveRangeOf_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create(10);
        eventInfo.MustBeInTheExclusiveRangeOf(1, 10);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessThanOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(0);
        eventInfo.MustBeLessThanOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessThanOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeLessThanOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessThanOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(1);
        eventInfo.MustBeLessThanOrBeNull(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessOrEqualToOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(1);
        eventInfo.MustBeLessOrEqualToOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessOrEqualToOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeLessOrEqualToOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeLessOrEqualToOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(2);
        eventInfo.MustBeLessOrEqualToOrBeNull(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterThanOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(2);
        eventInfo.MustBeGreaterThanOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterThanOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeGreaterThanOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterThanOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(1);
        eventInfo.MustBeGreaterThanOrBeNull(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterOrEqualToOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(1);
        eventInfo.MustBeGreaterOrEqualToOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterOrEqualToOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeGreaterOrEqualToOrBeNull(1);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeGreaterOrEqualToOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(0);
        eventInfo.MustBeGreaterOrEqualToOrBeNull(1);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheInclusiveRangeOfOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(5);
        eventInfo.MustBeInTheInclusiveRangeOfOrBeNull(1, 10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheInclusiveRangeOfOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeInTheInclusiveRangeOfOrBeNull(1, 10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheInclusiveRangeOfOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(11);
        eventInfo.MustBeInTheInclusiveRangeOfOrBeNull(1, 10);
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheExclusiveRangeOfOrBeNull_With_Valid_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(5);
        eventInfo.MustBeInTheExclusiveRangeOfOrBeNull(1, 10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheExclusiveRangeOfOrBeNull_With_Null_Value_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<int?>(null);
        eventInfo.MustBeInTheExclusiveRangeOfOrBeNull(1, 10);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void MustBeInTheExclusiveRangeOfOrBeNull_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<int?>(10);
        eventInfo.MustBeInTheExclusiveRangeOfOrBeNull(1, 10);
        eventInfo.Received().Error(Arg.Any<string>());
    }
}