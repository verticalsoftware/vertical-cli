using System.Collections;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a read-only view of the token list.
/// </summary>
public sealed class ReadOnlyTokenList : ITokenList
{
    internal ReadOnlyTokenList(IEnumerable<ArgumentToken> tokens)
    {
        var tokenArray = tokens
            .Select(token => new ArgumentToken(this, token.Kind, token.Text, token.Value, token.Symbol))
            .ToArray();
        
        TokenList.Link(tokenArray);

        var (first, last) = tokenArray.Length > 0 ? (tokenArray[0], tokenArray[^1]) : (null, null);
        
        First = first;
        Last = last;
        Count = tokenArray.Length;
    }

    /// <inheritdoc />
    public ArgumentToken? First { get; }

    /// <inheritdoc />
    public ArgumentToken? Last { get; }

    /// <inheritdoc />
    public int Count { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Count}";

    /// <inheritdoc />
    public IEnumerator<ArgumentToken> GetEnumerator() => new TokenList.Enumerator(First);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}