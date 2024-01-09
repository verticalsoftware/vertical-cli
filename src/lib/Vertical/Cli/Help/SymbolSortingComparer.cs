using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Help;

internal sealed class SymbolSortingComparer : IComparer<SymbolDefinition>
{
    /// <inheritdoc />
    public int Compare(SymbolDefinition? x, SymbolDefinition? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (ReferenceEquals(x, null))
            return -1;

        if (ReferenceEquals(y, null))
            return 1;

        var keyOfX = GetSortingIdentity(x);
        var keyOfY = GetSortingIdentity(y);

        return StringComparer.OrdinalIgnoreCase.Compare(keyOfX, keyOfY);
    }

    private static string GetSortingIdentity(SymbolDefinition symbol)
    {
        return symbol.Identities
            .Select(id => SymbolSyntax.Parse(id).SimpleIdentifiers![0])
            .OrderBy(id => id)
            .First();
    }
}