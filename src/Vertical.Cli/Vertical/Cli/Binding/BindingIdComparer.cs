namespace Vertical.Cli.Binding;

/// <summary>
/// A specialty comparer that evaluates symbol identifiers (e.g., --path) to parameter or property
/// names (e.g. path, Path).
/// </summary>
internal sealed class BindingIdComparer : IEqualityComparer<string>
{
    internal static BindingIdComparer Default { get; } = new();

    private BindingIdComparer()
    {
    }
    
    /// <inheritdoc />
    public bool Equals(string? first, string? second)
    {
        if (ReferenceEquals(first, second))
            return true;

        if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            return false;
        
        var spanX = first.AsSpan();
        var spanY = second.AsSpan();
        var (posX, posY) = (0, 0);
        var ignoreCase = true;

        while (posX < spanX.Length && posY < spanY.Length)
        {
            var (cx, cy) = (spanX[posX], spanY[posY]);

            if (!char.IsLetterOrDigit(cx))
            {
                ignoreCase = true;
                ++posX;
                continue;
            }

            if (!char.IsLetterOrDigit(cy))
            {
                ignoreCase = true;
                ++posY;
                continue;
            }

            switch (ignoreCase)
            {
                case false when cx != cy:
                case true when char.ToLower(cx) != char.ToLower(cy):
                    return false;
            }

            ++posX;
            ++posY;
            ignoreCase = false;
        }
        
        return true;
    }

    /// <inheritdoc />
    public int GetHashCode(string obj)
    {
        var hashCode = 0;

        foreach (var c in obj.Where(char.IsLetterOrDigit))
        {
            hashCode = HashCode.Combine(hashCode, char.ToLower(c));
        }
        
        return hashCode;
    }
}