using System.Text.RegularExpressions;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the structure of an argument.
/// </summary>
#if NETSTANDARD2_0
public sealed class ArgumentSyntax : IEquatable<ArgumentSyntax>
#else
public sealed partial class ArgumentSyntax : IEquatable<ArgumentSyntax>
#endif
{
    internal ArgumentSyntax(
        OptionPrefixType prefixType,
        string text,
        string identifierSymbol = "",
        string identifierName = "",
        string operandSymbol = "",
        string operandValue = "")
    {
        PrefixType = prefixType;
        Text = text;
        IdentifierSymbol = identifierSymbol;
        IdentifierName = identifierName;
        OperandSymbol = operandSymbol;
        OperandValue = operandValue;
    }

    /// <summary>
    /// Gets the prefix type.
    /// </summary>
    public OptionPrefixType PrefixType { get; }

    /// <summary>
    /// Gets the full text of the argument (original form).
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the full identifier symbol with prefix.
    /// </summary>
    public string IdentifierSymbol { get; }

    /// <summary>
    /// Gets the identifier name without prefix.
    /// </summary>
    public string IdentifierName { get; }

    /// <summary>
    /// Gets the operand symbol or empty string.
    /// </summary>
    public string OperandSymbol { get; }

    /// <summary>
    /// Gets the operand value or empty string.
    /// </summary>
    public string OperandValue { get; }

    /// <summary>
    /// Parses the structure of an argument.
    /// </summary>
    /// <param name="text">The argument to parse.</param>
    /// <returns><see cref="ArgumentSyntax"/></returns>
    public static ArgumentSyntax Parse(string text)
    {
        var match = MyRegex().Match(text);
        var prefix = match.Groups[2].Value switch
        {
            "-" => OptionPrefixType.PosixOption,
            "--" => OptionPrefixType.GnuOption,
            _ => OptionPrefixType.None
        };
        var hasIdentifier = prefix != OptionPrefixType.None;

        return new ArgumentSyntax(
            prefix,
            match.Value,
            hasIdentifier ? match.Groups[1].Value : string.Empty,
            hasIdentifier ? match.Groups[3].Value : string.Empty,
            match.Groups[4].Value,
            match.Groups[5].Value);
    }

#if NET8_0_OR_GREATER
    [GeneratedRegex(@"^((--?)?([^:=]*))(?:([:=])(.*))?$")]
    private static partial Regex MyRegex();
#else
    private static readonly Regex RegexInstance = new Regex("^((--?)?([^:=]*))(?:([:=])(.*))?$");
    private static Regex MyRegex() => RegexInstance;
#endif    

    /// <inheritdoc />
    public bool Equals(ArgumentSyntax? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return PrefixType == other.PrefixType 
               && Text == other.Text 
               && IdentifierSymbol == other.IdentifierSymbol 
               && IdentifierName == other.IdentifierName 
               && OperandSymbol == other.OperandSymbol 
               && OperandValue == other.OperandValue;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ArgumentSyntax other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Text.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Text;
}

