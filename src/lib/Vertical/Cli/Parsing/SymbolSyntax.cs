using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the syntax of a symbol.
/// </summary>
public sealed class SymbolSyntax : IEquatable<SymbolSyntax>
{
    internal SymbolSyntax(
        SymbolSyntaxType type,
        string text,
        string prefix = "",
        string[]? identifiers = null,
        char operandAssignmentToken = '\0',
        string operandValue = "")
    {
        Type = type;
        Text = text;
        Prefix = prefix;
        Identifiers = identifiers ?? Array.Empty<string>();
        OperandAssignmentToken = operandAssignmentToken;
        OperandValue = operandValue;
    }

    /// <summary>
    /// Parses the symbol.
    /// </summary>
    /// <param name="str">String to parse.</param>
    /// <returns><see cref="SymbolSyntax"/></returns>
    public static SymbolSyntax Parse(string str) => SymbolSyntaxParser.Parse(str);

    /// <summary>
    /// Gets whether the symbol contains a single identifier.
    /// </summary>
    public bool HasSingleIdentifier => Identifiers.Length == 1;

    /// <summary>
    /// Gets whether the symbol contains a valid identifier.
    /// </summary>
    public bool HasIdentifiers => Identifiers.Length > 0;

    /// <summary>
    /// Gets whether the syntax is prefixed.
    /// </summary>
    public bool IsPrefixed => Prefix.Length > 0;

    /// <summary>
    /// Gets whether the symbol contains an operand value.
    /// </summary>
    public bool HasOperand => OperandAssignmentToken != char.MinValue && OperandValue.Length > 0;

    /// <summary>
    /// Gets the prefix format.
    /// </summary>
    public SymbolSyntaxType Type { get; }
    
    /// <summary>
    /// Gets the full text.
    /// </summary>
    public string Text { get; }
    
    /// <summary>
    /// Gets the prefix portion of the symbol - returns an empty string if there is
    /// no prefix.
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// Gets the identifiers of the symbol, which is any part of the text that excludes an operand expression
    /// when this syntax represents an identifier.
    /// </summary>
    public string[] Identifiers { get; }

    /// <summary>
    /// Gets the operand assignment token, or <c>char.MinValue</c>.
    /// </summary>
    public char OperandAssignmentToken { get; }
    
    /// <summary>
    /// Gets the operand value, or an empty string.
    /// </summary>
    public string OperandValue { get; }

    /// <summary>
    /// Gets the operand expression (assignment token and value).
    /// </summary>
    public string OperandExpression => OperandAssignmentToken != char.MinValue
        ? $"{OperandAssignmentToken}{OperandValue}"
        : string.Empty;

    /// <inheritdoc />
    public bool Equals(SymbolSyntax? other) => Text.Equals(other?.Text);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is SymbolSyntax other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Text.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Text;

    public static bool operator ==(SymbolSyntax x, SymbolSyntax y) => x.Equals(y);
    public static bool operator !=(SymbolSyntax x, SymbolSyntax y) => !x.Equals(y);
}