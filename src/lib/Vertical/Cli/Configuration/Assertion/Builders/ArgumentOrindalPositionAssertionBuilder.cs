using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class AmbiguousArgumentOrdinalPositionsBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.CallSites)
        {
            ValidateArgumentPositions(context, command);
        }   
    }

    private static void ValidateArgumentPositions(AssertionContext context, Command command)
    {
        var arguments = context.GetPositionArguments(command);
        var groups = arguments
            .GroupBy(argument => argument.OrdinalPosition)
            .Where(group => group.Count() > 1);

        context
            .Assertions
            .AddRange(groups
                .Select(group => new ArgumentOrdinalPositionAssertion(command, group.ToArray())));
    }
}