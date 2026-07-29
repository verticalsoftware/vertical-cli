namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents an argument token.
/// </summary>
public sealed class ArgumentToken
{
    internal ArgumentToken(
        ITokenList tokenList,
        TokenKind kind,
        string text,
        string? value,
        string? symbol = null)
    {
        TokenList = tokenList;
        Kind = kind;
        Text = text;
        Symbol = symbol;
        Value = value;
    }

    /// <summary>
    /// Gets the list the token belongs to.
    /// </summary>
    public ITokenList? TokenList { get; internal set; }

    /// <summary>
    /// Gets the token kind.
    /// </summary>
    public TokenKind Kind { get; }

    /// <summary>
    /// Gets the text of the argument.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the symbol of an option or directive.
    /// </summary>
    public string? Symbol { get; }

    /// <summary>
    /// Gets one of the following:
    /// - The text of an argument
    /// - The parameter value of an option or directive
    /// - The value of an annotation
    /// </summary>
    public string? Value { get; }
    
    /// <summary>
    /// Gets the token that precedes this instance.
    /// </summary>
    public ArgumentToken? Previous { get; internal set; }
    
    /// <summary>
    /// Gets the token that follows this instance.
    /// </summary>
    public ArgumentToken? Next { get; internal set; }

    /// <inheritdoc />
    public override string ToString() => Symbol != null
        ? $"({Kind}) {Symbol} = '{Text}'"
        : $"({Kind}) {Text}";

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Kind, Text, Symbol);
}