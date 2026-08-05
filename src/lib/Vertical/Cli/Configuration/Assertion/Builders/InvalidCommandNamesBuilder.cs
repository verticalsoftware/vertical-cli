using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal class InvalidCommandNamesBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        context
            .Assertions
            .AddRange(context
                .Commands
                .Where(command => ArgumentSyntax.GetSyntaxKind(command.Name) != SyntaxKind.None)
                .Select(command => new InvalidCommandNameAssertion(command)));
    }
}