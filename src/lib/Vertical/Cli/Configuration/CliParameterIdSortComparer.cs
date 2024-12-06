namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a comparer for <see cref="CliParameter"/> that sorts based on identifier.
/// </summary>
public sealed class CliParameterIdSortComparer : IComparer<CliParameter>
{
    /// <summary>
    /// Defines an instance of this type.
    /// </summary>
    public static readonly IComparer<CliParameter> Instance = new CliParameterIdSortComparer();
    
    /// <inheritdoc />
    public int Compare(CliParameter? x, CliParameter? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (ReferenceEquals(x, null))
            return -1;

        if (ReferenceEquals(y, null))
            return 1;

        var spanX = TrimIdentifier(x);
        var spanY = TrimIdentifier(y);
        var max = Math.Min(spanX.Length, spanY.Length);
        var comparer = Comparer<char>.Default;
        
        for (var c = 0; c < max; c++)
        {
            var cmp = comparer.Compare(char.ToLower(spanX[c]), char.ToLower(spanY[c]));
            if (cmp != 0)
            {
                return cmp;
            }
        }

        return spanX.Length == spanY.Length
            ? 0
            : spanX.Length > spanY.Length
                ? 1
                : -1;
    }

    private static ReadOnlySpan<char> TrimIdentifier(CliParameter cliParameter)
    {
        var span = cliParameter.Identifiers[0].Text.AsSpan();

        while (span.Length > 0 && span[0] == '-')
            span = span[1..];

        return span;
    }
}