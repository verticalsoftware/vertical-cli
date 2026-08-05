using NSubstitute;
using Vertical.Cli.Validation;

namespace Vertical.Cli.UnitTests.Validation;

public static class ValidationHelpers
{
    public static IValidationEventInfo<object, TValue> Create<TValue>(TValue value)
    {
        var mock = Substitute.For<IValidationEventInfo<object, TValue>>();
        mock.Value.Returns(value);
        mock.OK.Returns(mock);
        return mock;
    }
}