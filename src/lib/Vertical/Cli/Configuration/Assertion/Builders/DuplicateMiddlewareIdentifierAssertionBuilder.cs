using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class DuplicateMiddlewareIdentifierAssertionBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        context
            .Assertions
            .AddRange(context
                .GetMiddlewareIdentifierLookup()
                .Where(grouping => grouping.Count() > 1)
                .Select(grouping => new DuplicateMiddlewareIdentifierAssertion(
                    grouping.Key,
                    grouping.ToArray())));
    }
}