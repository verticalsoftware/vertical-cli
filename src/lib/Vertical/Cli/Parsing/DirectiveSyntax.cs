using System.Diagnostics.CodeAnalysis;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the syntax of a directive.
/// </summary>
public readonly struct DirectiveSyntax : IParsable<DirectiveSyntax>
{
    private DirectiveSyntax(string text, int operandPosition)
    {
        _operandPosition = operandPosition;
        Text = text;
    }

    private readonly int _operandPosition;

    /// <summary>
    /// Gets the full token text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the full token text as a span.
    /// </summary>
    public ReadOnlySpan<char> TextSpan => Text.AsSpan();

    /// <summary>
    /// Gets the text enclosed in square brackets.
    /// </summary>
    public ReadOnlySpan<char> EnclosedSpan => TextSpan[1..^1];

    /// <summary>
    /// Gets the directive identifier.
    /// </summary>
    public ReadOnlySpan<char> IdentifierSpan => _operandPosition > -1
        ? EnclosedSpan[.._operandPosition]
        : EnclosedSpan;

    /// <summary>
    /// Gets the parameter span including the identifier terminator character.
    /// </summary>
    public ReadOnlySpan<char> ParameterSyntaxSpan => _operandPosition > -1
        ? EnclosedSpan[_operandPosition..]
        : ReadOnlySpan<char>.Empty;

    /// <summary>
    /// Gets the parameter span.
    /// </summary>
    public ReadOnlySpan<char> ParameterSpan => _operandPosition > -1
        ? ParameterSyntaxSpan[1..]
        : ReadOnlySpan<char>.Empty;

    /// <summary>
    /// Gets whether the directive has a parameter span.
    /// </summary>
    public bool HasParameterSpan => _operandPosition > -1;

    /// <summary>
    /// Parses a directive token.
    /// </summary>
    /// <param name="text">The directive token text.</param>
    /// <returns><see cref="DirectiveSyntax"/></returns>
    /// <exception cref="ArgumentException"><paramref name="text"/> is not a directive syntax token</exception>
    public static DirectiveSyntax Parse(string text)
    {
        var span = text.AsSpan();

        if (span is not ['[', .., ']'])
        {
            throw new ArgumentException($"Token value '{text}' is not a directive");
        }

        return new DirectiveSyntax(text, span[1..^1].IndexOfAny([':', '=']));
    }

    /// <inheritdoc />
    public override string ToString() => Text;

    /// <inheritdoc />
    public static DirectiveSyntax Parse(string text, IFormatProvider? provider)
    {
        return TryParse(text, provider, out var result)
            ? result
            : throw new ArgumentException($"Invalid directive format '{text}'");
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? text, 
        IFormatProvider? provider, 
        out DirectiveSyntax result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        var span = text.AsSpan();
        var isDirective = span is ['[', .., ']'];

        result = isDirective
            ? new DirectiveSyntax(text, span[1..^1].IndexOfAny([':', '=']))
            : default;

        return isDirective;
    }
}