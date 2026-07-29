namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the basic interface of a token list.
/// </summary>
public interface ITokenList : IEnumerable<ArgumentToken>
{
    /// <summary>
    /// Gets the first token in the list.
    /// </summary>
    ArgumentToken? First { get; }

    /// <summary>
    /// Gets the last token in the list.
    /// </summary>
    ArgumentToken? Last { get; }

    /// <summary>
    /// Gets the number of tokens in the list.
    /// </summary>
    int Count { get; }
}