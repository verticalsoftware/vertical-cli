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

    public static IValidationEventInfo<object, TElement, TCollection> Create<TElement, TCollection>(
        TCollection value)
        where TCollection : IEnumerable<TElement>
    {
        var mock = Substitute.For<IValidationEventInfo<object, TElement, TCollection>>();
        mock.Value.Returns(value);
        mock.OK.Returns(mock);
        mock
            .When(x => x.EachValue(Arg.Any<Action<IValidationEventInfo<object, TElement>>>()))
            .Do(callInfo =>
            {
                var action = callInfo.ArgAt<Action<IValidationEventInfo<object, TElement>>>(0);
                foreach (var element in value)
                {
                    var valueContext = Create(element);
                    
                    valueContext
                        .When(x => x.Error(Arg.Any<string>()))
                        .Do(_ => mock.Error(string.Empty));
                    
                    action(valueContext);
                }
            });

        return mock;
    }
}