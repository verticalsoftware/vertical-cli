namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a breakdown of a string's syntax in the context of a <see cref="Token"/>
/// </summary>
public sealed class TokenSyntax : IEquatable<TokenSyntax>
{
    private TokenSyntax(SyntaxKind kind, string text, int operandPosition)
    {
        _operandPosition = operandPosition;
        
        Kind = kind;
        Text = text;
    }

    private readonly int _operandPosition;

    /// <summary>
    /// Gets the syntax kind.
    /// </summary>
    public SyntaxKind Kind { get; }

    /// <summary>
    /// Gets the original text that was parsed.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the span of the original text that was parsed.
    /// </summary>
    public ReadOnlySpan<char> TextSpan => Text.AsSpan();

    /// <summary>
    /// Evaluates the given string and construct a syntax object.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <returns><see cref="TokenSyntax"/></returns>
    public static TokenSyntax Parse(string str)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(str);

        var span = str.AsSpan();

        switch (span)
        {
            case ['-', '-']:
                return new TokenSyntax(SyntaxKind.Terminator, str, -1);
            
            case ['-', '-', ..]:
            case ['-', ..]:
            case ['/', ..]:
                return new TokenSyntax(SyntaxKind.PrefixedIdentifier, str, span.IndexOfAny([':', '=']));
            
            case ['[', .., ']']:
                return new TokenSyntax(SyntaxKind.EnclosedSymbol, str, span[1..^1].IndexOfAny([':', '=']));
            
            default:
                return new TokenSyntax(SyntaxKind.NonDecorated, str, -1);
        }
    }

    /// <summary>
    /// Gets the portion of the text span enclosed by brackets. If this value is not an enclosed symbol,
    /// an empty span is returned.
    /// </summary>
    public ReadOnlySpan<char> EnclosedSpan => Kind == SyntaxKind.EnclosedSymbol
        ? TextSpan[1..^1]
        : ReadOnlySpan<char>.Empty;

    /// <summary>
    /// Gets a span that represents the identifier portion of a prefixed symbol with the parameter syntax
    /// excluded. If the value is not an identifier, an empty span is returned.
    /// </summary>
    public ReadOnlySpan<char> SymbolSpan => this switch
    {
        { Kind: SyntaxKind.PrefixedIdentifier, _operandPosition: not -1 } => TextSpan[.._operandPosition],
        { Kind: SyntaxKind.PrefixedIdentifier } => TextSpan,
        _ => ReadOnlySpan<char>.Empty
    };

    /// <summary>
    /// Gets the symbol convention.
    /// </summary>
    public SymbolConvention? SymbolConvention => Kind == SyntaxKind.PrefixedIdentifier
        ? this switch
        {
            { SymbolPrefixSpan: ['-','-'] } => Parsing.SymbolConvention.GnuOption,
            { SymbolPrefixSpan: ['-'], NonPrefixedSymbolSpan.Length: > 1 } => Parsing.SymbolConvention.PosixGroup,
            { SymbolPrefixSpan: ['-'] } => Parsing.SymbolConvention.PosixOption,
            _ => Parsing.SymbolConvention.ForwardSlashOption
        }
        : null;

    /// <summary>
    /// Gets a span that represents the identifier portion of a prefixed symbol with the parameter syntax
    /// and the prefix character(s) excluded. If the value is not an identifier, an empty span is returned.
    /// </summary>
    public ReadOnlySpan<char> NonPrefixedSymbolSpan => Kind == SyntaxKind.PrefixedIdentifier
        ? SymbolPrefixSpan switch
        {
            ['-', '-'] => SymbolSpan[2..],
            ['-'] => SymbolSpan[1..],
            ['/'] => SymbolSpan[1..],
            _ => ReadOnlySpan<char>.Empty
        }
        : ReadOnlySpan<char>.Empty;

    /// <summary>
    /// Gets the span that represents the prefix character(s) of a symbol. If the value is not an identifier, an
    /// empty span is returned.
    /// </summary>
    public ReadOnlySpan<char> SymbolPrefixSpan => Kind == SyntaxKind.PrefixedIdentifier
        ? TextSpan switch
        {
            ['-', '-', ..] => ['-', '-'],
            ['-', ..] => ['-'],
            ['/', ..] => ['/'],
            _ => ReadOnlySpan<char>.Empty
        }
        : ReadOnlySpan<char>.Empty;

    /// <summary>
    /// Gets whether the token has a non-zero length attached parameter value.
    /// </summary>
    public bool HasParameter => Kind == SyntaxKind.PrefixedIdentifier && _operandPosition > -1;

    /// <summary>
    /// Gets the span that represents the full syntax of the attached parameter including the assignment
    /// operator. If this value is not a symbol or there is no operand, an empty span is returned.
    /// </summary>
    public ReadOnlySpan<char> ParameterSyntaxSpan => HasParameter
        ? TextSpan[_operandPosition..]
        : ReadOnlySpan<char>.Empty;

    /// <summary>
    /// Gets the character used to join the symbol to the parameter value.
    /// </summary>
    public char? ParameterAssignmentOperator => HasParameter
        ? TextSpan[_operandPosition]
        : null;

    /// <summary>
    /// Gets the span of the attached parameter.
    /// </summary>
    public ReadOnlySpan<char> ParameterSpan => HasParameter
        ? TextSpan[(_operandPosition + 1)..]
        : ReadOnlySpan<char>.Empty;

    /// <inheritdoc />
    public override string ToString() => Text;

    /// <inheritdoc />
    public bool Equals(TokenSyntax? other) => Text.Equals(other?.Text);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is TokenSyntax other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Text.GetHashCode();
}