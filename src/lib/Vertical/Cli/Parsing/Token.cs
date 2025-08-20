using System.Diagnostics.CodeAnalysis;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents an argument token, which describes the properties of a string argument
/// that can be formed into a contextual meaning to the command line.
/// </summary>
public sealed class Token : IEquatable<Token>
{
    internal Token(TokenKind kind, TokenSyntax syntax, int parsePosition)
    {
        Kind = kind;
        Syntax = syntax;
        ParsePosition = parsePosition;
    }

    /// <summary>
    /// Gets the token kind.
    /// </summary>
    public TokenKind Kind { get; }

    /// <summary>
    /// Gets the syntax of the token.
    /// </summary>
    public TokenSyntax Syntax { get; }

    /// <summary>
    /// Gets the parse position.
    /// </summary>
    public int ParsePosition { get; private set; }

    /// <summary>
    /// Gets the text of the token.
    /// </summary>
    public string Text => Syntax.Text;

    /// <summary>
    /// Gets the symbol identifier.
    /// </summary>
    public string Symbol => Syntax.SymbolSpan.ToString();

    /// <summary>
    /// Gets the parameter value.
    /// </summary>
    public string ParameterValue => Syntax.ParameterSpan.ToString();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool Equals(Token? other)
    {
        return other != null
               && Text.Equals(other.Text)
               && ParsePosition == other.ParsePosition
               && Kind == other.Kind;
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => Text;
}