using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class InvalidAliasesBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        var invalidEntries = context
            .CallSites
            .SelectMany(command => context
                .GetModelConfiguration(command.ModelType!)
                .BindingSources
                .OfType<CliSymbol>()
                .Where(symbol => symbol.Kind is SymbolKind.Option or SymbolKind.Switch)
                .SelectMany(symbol => symbol.Aliases.Select(alias => (alias, symbol)))
                .Where(item => ArgumentSyntax.GetSyntaxKind(item.alias) != SyntaxKind.Option));

        context
            .Assertions
            .AddRange(invalidEntries
                .GroupBy(item => item.symbol.ModelType)
                .SelectMany(modelGroup => modelGroup
                    .GroupBy(item => item.symbol)
                    .Select(symbolGroup => new InvalidAliasAssertion(
                        modelGroup.Key,
                        symbolGroup.Key,
                        symbolGroup.Select(item => item.alias).ToArray()))));
    }
}