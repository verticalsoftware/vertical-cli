namespace Vertical.Cli.Internal;

internal sealed class CaseInsensitiveCharComparer : IEqualityComparer<char>
{
    public static readonly CaseInsensitiveCharComparer Default = new();
    
    /// <inheritdoc />
    public bool Equals(char x, char y) => char.ToLower(x).Equals(char.ToLower(y));

    /// <inheritdoc />
    public int GetHashCode(char obj) => obj.GetHashCode();
}