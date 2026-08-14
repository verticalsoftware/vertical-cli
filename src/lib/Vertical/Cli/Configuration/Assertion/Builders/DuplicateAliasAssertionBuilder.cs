using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class DuplicateAliasAssertionBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.CallSites)
        {
            ValidateUniqueAliases(context, command);
        }
    }

    private static void ValidateUniqueAliases(AssertionContext context, Command command)
    {
        var options = context.GetNamedSymbols(command);
        
        context
            .Assertions
            .AddRange(options
                .SelectMany(symbol => symbol.Aliases.Select(alias => (alias, symbol)))
                .GroupBy(item => item.alias)
                .Where(group => group.Count() > 1)
                .Select(group => new DuplicateAliasAssertion(command, 
                    group.Key,
                    group.Select(item => item.symbol).ToArray())));
    }
}