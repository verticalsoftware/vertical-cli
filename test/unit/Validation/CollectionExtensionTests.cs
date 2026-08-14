using NSubstitute;
using Shouldly;
using Vertical.Cli.Validation;

namespace Vertical.Cli.UnitTests.Validation;

public class CollectionExtensionTests
{
    [Fact]
    public void Empty_Event_Info_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string, string[]>([]);
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }

    [Fact]
    public void EachValue_With_Valid_Values_Reports_Nothing()
    {
        var eventInfo = ValidationHelpers.Create<string, string[]>(["red", "green", "blue"]);
        eventInfo.EachValue(value => value.MustBeOneOf(["red", "green", "blue"]));
        eventInfo.DidNotReceive().Error(Arg.Any<string>());
    }
    
    [Fact]
    public void EachValue_With_Invalid_Value_Reports_Error()
    {
        var eventInfo = ValidationHelpers.Create<string, string[]>(["pink", "green", "blue"]);
        eventInfo.EachValue(value => value.MustBeOneOf(["red", "green", "blue"]));
        eventInfo.Received().Error(Arg.Any<string>());
    }

    [Fact]
    public void EachValue_Forwards_Value_Context()
    {
        var eventInfo = new ValidationEventInfo<object, string, string[]>(
            null!,
            null!,
            null!,
            ["red", "green", "blue"]);

        var queue = new Queue<string>(["red", "green", "blue"]);

        eventInfo.EachValue(valueContext => valueContext.Value.ShouldBe(queue.Dequeue()));
    }
}